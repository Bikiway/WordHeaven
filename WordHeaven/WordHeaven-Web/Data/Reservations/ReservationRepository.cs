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

        public async Task<Reservation> GetReservationIdAsync(int Id)
        {
            return await _context.Reservations.FindAsync(Id);
        }

        public async Task<IQueryable<ReservationDetailsTemp>> GetReservationTempAsync(string userName)
        {
           var user = await _userHelper.GetUserByEmailAsync(userName);
            if(user == null) { return null;}

            return _context.ReservationsDetailTemp
                .Include(o => o.StoreName)
                .Include(o => o.book)
                .Where(u => u.user == user)
                .OrderBy(o => o.Id);
        }

        public Task<int[]> GetStoresID(int Id)
        {
            throw new System.NotImplementedException();
        }

        public Task<int[]> RenewReservationLoan(int Id, string userName)
        {
            throw new System.NotImplementedException();
        }

        public async Task ReservationCompleted(ReservationCompletedViewModel model)
        {
            var reserve = await _context.Reservations.FindAsync(model.Id);

            if(reserve == null)
            {
                return;
            }

            reserve.Id = model.Id;
            _context.Reservations.Update(reserve);
            await _context.SaveChangesAsync();
        }

        public Task<int[]> ReservationOutOfTime(int Id)
        {
            throw new System.NotImplementedException();
        }
    }
}
