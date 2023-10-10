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
                    BookCover = model.BookCoverId,
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

            var detailsInfo = reserveTemps.Select(o => new ReservationDetails
            {
                StoreName = o.StoreName,
                bookName = o.book,
                BookCover = o.BookCover,
                Request = o.Request,
                BookReturned = o.BookReturned,
                LoanedBook = o.LoanedBook,
            }).ToList();

            int imageId = reserveTemps.FirstOrDefault()?.book.Id ?? 0;
            var image = GetBookCover(imageId);

            var reservations = new Reservation
            {
                StoreName = detailsInfo.FirstOrDefault()?.StoreName,
                ClientFirstName = detailsInfo.FirstOrDefault().ClientFirstName,
                ClientLastName = detailsInfo.FirstOrDefault()?.ClientLastName,
                BookName = detailsInfo.FirstOrDefault()?.bookName.Title,
                BookCover = (await image)[0],
                IsBooked = true,
                BookReturned = detailsInfo.FirstOrDefault().BookReturned,
                LoanedBook = detailsInfo.FirstOrDefault().LoanedBook,
                user = user,
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
            var image = await _context.Books.FirstOrDefaultAsync(b => b.Id == Id);
          
            if (image != null)
            {
                return new byte[] { Convert.ToByte(image.BookCover) };
            }

            return null;
        }

        public async Task<IQueryable<Reservation>> GetReservationAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if(user == null)
            {
                return null;
            }

            if(await _userHelper.IsUserInRoleAsync(user, "Employee"))
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

        public async Task<DateTime> RenewReservationLoan(int Id, string userName)
        {
            var Renew = await _context.Reservations
                .Where(s => s.Id == Id)
                .Select(s => s.RenewBookLoan.AddDays(20))
                .FirstOrDefaultAsync();

            return Renew;
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
                var limitAsPassed = await  _context.Reservations
                    .Where(l => l.Id == Id)
                    .Select(l => l.LoanTimeLimit)
                    .FirstOrDefaultAsync();

                var tax = await _context.Reservations
                    .Where(t => t.Id == Id)
                    .Select(t => t.PayTaxesLoan == 1)
                    .FirstOrDefaultAsync();

                var turnedIn = await _context.Reservations
                    .Where(t => t.Id == Id)
                    .Select(t => t.BookReturnedByClient)
                    .FirstOrDefaultAsync();

                if(turnedIn.Equals(true))
                {
                    extra = DateTime.Today;

                    TimeSpan difference = extra - limitAsPassed;

                    int total = difference.Days + Convert.ToInt32(tax);

                    return total;
                }
            }

            return 0;

        }
    }
}
