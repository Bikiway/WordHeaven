using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Data.Stores;
using WordHeaven_Web.Helpers;
using WordHeaven_Web.Models.Reservation;

namespace WordHeaven_Web.Data.Reservations
{
    public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IStoreRepository _storeRepository;
        public ReservationRepository(DataContext context, IStoreRepository storeRepository, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _storeRepository = storeRepository;
        }

        public async Task AddItemToReservationAsync(AddReservationViewModel model, string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null) { return; }

            var books = await _context.Books.FindAsync(model.BookId);
            var store = await _context.Stores.FindAsync(model.storeId);
            byte imageId = Convert.ToByte(books.BookCover);
            byte[] image = await GetBookCover(Convert.ToSByte(imageId));

            if (books == null && store == null)
            { return; }

            var reserveDetailTemp = await _context.ReservationsDetailTemp
                .Where(rdt => rdt.user == user && rdt.book == books)
                .Where(rdt => rdt.user == user && rdt.StoreName == store)
                .FirstOrDefaultAsync();

            if (reserveDetailTemp == null)
            {
                reserveDetailTemp = new ReservationDetailsTemp
                {
                    StoreName = store,
                    ClientFirstName = model.ClientFirstName,
                    ClientLastName = model.ClientLastName,
                    book = books,
                    BookCover = image,
                    BookReturned = model.BookReturned,
                    LoanedBook = model.LoanedBook,
                    IsBooked = model.IsBooked,
                    Request = model.Request,
                    UserName = model.UserName,
                    user = user
                };
                _context.ReservationsDetailTemp.Add(reserveDetailTemp);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ClientDidntReturnBook(int Id)
        {
            var Notreturned = await _context.Reservations
             .Where(x => x.Id == Id)
             .Select(x => x.ClientDidntReturnTheBook)
             .FirstOrDefaultAsync();

            return Notreturned;
        }

        public async Task<bool> ClientReturnedTheBook(int Id)
        {
            var returned = await _context.Reservations
                .Where(x => x.Id == Id)
                .Select(x => x.BookReturnedByClient)
                .FirstOrDefaultAsync();

            return returned;
        }

        public async Task<bool> ConfirmReservationAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null) { return false; }

            var reserveTemps = await _context.ReservationsDetailTemp
                .Include(r => r.StoreName)
                .Include(r => r.book)
                .Include(r => r.BookCover)
                .Where(r => r.user == user)
                .ToListAsync();

            if (reserveTemps == null || reserveTemps.Count == 0)
            {
                return false;
            }

            int Id = reserveTemps.FirstOrDefault()?.book.Id ?? 0;
            var image = GetBookCover(Id);
            var renewed = RenewReservationLoan(Id);
            var reservationOutOfTime = ReservationOutOfTime(Id, DateTime.Today);
            var ClientDidntReturned = ClientDidntReturnBook(Id);
            var bookReturned = ClientReturnedTheBook(Id);
            var TimeLimit = LoanTimeLimit(Id);
            var Payed = TaxesPayedByClient(Id);
            var isBooked = IsBookReturned(Id);


            var detailsInfo = reserveTemps.Select(o => new ReservationDetails
            {
                StoreName = o.StoreName,
                bookName = o.book,
                BookCover = image.Result,
                Request = o.Request,
                BookReturned = o.BookReturned,
                LoanedBook = o.LoanedBook,
                UserName = o.UserName,
            }).ToList();

            var reservations = new Reservation
            {
                StoreName = detailsInfo.FirstOrDefault()?.StoreName,
                ClientFirstName = detailsInfo.FirstOrDefault().ClientFirstName,
                ClientLastName = detailsInfo.FirstOrDefault()?.ClientLastName,
                BookName = detailsInfo.FirstOrDefault()?.bookName.Title,
                BookCover = (await image)[0],
                IsBooked = isBooked.Result,
                BookReturned = detailsInfo.FirstOrDefault().BookReturned,
                LoanedBook = detailsInfo.FirstOrDefault().LoanedBook,
                RenewBookLoan = renewed.Result,
                BookReturnedByClient = bookReturned.Result,
                PayTaxesLoan = reservationOutOfTime.Result,
                LoanTimeLimit = DateTime.FromOADate(TimeLimit.Result),
                PayedTaxesLoan = Payed.Result,
                ClientDidntReturnTheBook = ClientDidntReturned.Result,
                user = user,
                UserName = detailsInfo.FirstOrDefault()?.UserName,
                Items = detailsInfo,
            };

            await CreateAsync(reservations);
            _context.ReservationsDetailTemp.RemoveRange(reserveTemps);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task DeleteTempAsync(int Id)
        {
            var delete = await _context.ReservationsDetailTemp.FindAsync(Id);

            if (delete == null)
            {
                return;
            }

            _context.ReservationsDetailTemp.Remove(delete);
            await _context.SaveChangesAsync();
        }

        public async Task<byte[]> GetBookCover(int Id)
        {
            var image = await _context.Books
                .Where(i => i.Id == Id)
                .Select(i => i.BookCover)
                .FirstOrDefaultAsync();

            return image;
        }

        public async Task<IQueryable<Reservation>> GetReservationAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Employee"))
            {
                return _context.Reservations
                    .Include(o => o.user)
                    .Include(o => o.Items)
                    .ThenInclude(s => s.StoreName)
                    .Include(o => o.Items)
                    .ThenInclude(b => b.bookName)
                    .OrderBy(o => o.Id);
            }

            return _context.Reservations
                    .Include(o => o.Items)
                    .ThenInclude(s => s.StoreName)
                    .Include(o => o.Items)
                    .ThenInclude(b => b.bookName)
                    .Where(u => u.user == user)
                    .OrderBy(o => o.Id);
        }

        public async Task<IQueryable<ReservationDetailsTemp>> GetReservationTempAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null) { return null; }

            return _context.ReservationsDetailTemp
                .Include(o => o.StoreName)
                .Include(o => o.book)
                .Where(u => u.user == user)
                .OrderBy(o => o.Id);
        }

        public async Task<string> GetStoresID(int Id)
        {
            var store = await _context.Stores
                .Where(s => s.Id == Id)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            return store;
        }

        public async Task<bool> IsBookReturned(int Id)
        {
            var book = await _context.Reservations
                .Where(s => s.Id == Id)
                .Select(s => s.BookReturnedByClient)
                .FirstOrDefaultAsync();
            
            if(book)
            {
                var returned = await _context.Reservations
                    .Where (s => s.Id == Id)
                    .Select(s => s.IsBooked)
                    .FirstOrDefaultAsync();

                return returned;
            }

            return false;
        }

        public async Task<int> LoanTimeLimit(int Id)
        {
            var br = await _context.Reservations
                .Where(i => i.Id == Id)
                .Select(i => i.BookReturned)
                .FirstOrDefaultAsync();

            DateTime extra = DateTime.Today;

            TimeSpan difference = br - extra;

            int ThreeDaysLeft = 3;

            var total = difference.Days == ThreeDaysLeft ? 1 : 0;
             
            return total;
        }

        public async Task ModifyStatusReservation(AlterStatusReservationViewModel model)
        {
            var status = await _context.Reservations.FindAsync(model.Id);

            if(status == null)
            {
                return;
            }

            status.Id = model.Id;
            _context.Reservations.Update(status);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RenewReservationLoan(int Id)
        {
            var Renew = await _context.Reservations
                .Where(s => s.Id == Id)
                .Select(s => s.RenewBookLoan)
                .FirstOrDefaultAsync();

            if(Renew == true)
            {
                var final = await _context.Reservations
                    .Where(f => f.Id == Id)
                    .Select(f => f.BookReturned.AddDays(20))
                    .FirstOrDefaultAsync();
            }

            return Renew = false;
        }

        public async Task ReservationCompleted(ReservationCompletedViewModel model)
        {
            var reserve = await _context.Reservations.FindAsync(model.Id);

            if (reserve == null)
            {
                return;
            }

            reserve.Id = model.Id;
            _context.Reservations.Update(reserve);
            await _context.SaveChangesAsync();
        }

        public async Task<int> ReservationOutOfTime(int Id, DateTime extra)
        {
            var limitPassed = await _context.Reservations
                .Where(l => l.Id == Id)
                .Select(l => l.ClientDidntReturnTheBook)
                .FirstOrDefaultAsync();

            if (limitPassed == true)
            {
                var limitAsPassed = await _context.Reservations
                    .Where(l => l.Id == Id)
                    .Select(l => l.BookReturned)
                    .FirstOrDefaultAsync();

                var tax = await _context.Reservations
                    .Where(t => t.Id == Id)
                    .Select(t => t.PayTaxesLoan == 1)
                    .FirstOrDefaultAsync();

                var turnedIn = await _context.Reservations
                    .Where(t => t.Id == Id)
                    .Select(t => t.BookReturnedByClient)
                    .FirstOrDefaultAsync();

                if (turnedIn.Equals(true))
                {
                    extra = DateTime.Today;

                    TimeSpan difference = extra - limitAsPassed;

                    int total = difference.Days + Convert.ToInt32(tax);

                    return total;
                }
            }

            return 0;

        }

        public async Task<bool> TaxesPayedByClient(int Id)
        {
            var taxPay = await _context.Reservations
                .Where (t => t.Id == Id)
                .Select (t => t.PayedTaxesLoan)
                .FirstOrDefaultAsync();

            return taxPay;
        }
    }
}
