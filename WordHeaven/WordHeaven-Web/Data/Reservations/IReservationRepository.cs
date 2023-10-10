using System.Linq;
using System.Threading.Tasks;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Models.Reservation;

namespace WordHeaven_Web.Data.Reservations
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task DeleteTempAsync(int Id);

        Task<bool> ConfirmReservationAsync(string userName);

        Task<IQueryable<ReservationDetailsTemp>> GetReservationTempAsync(string userName);

        Task AddItemToReservationAsync(AddReservationViewModel model, string userName);

        Task<IQueryable<Reservation>> GetReservationAsync(string userName);

        Task ReservationCompleted(ReservationCompletedViewModel model);

        Task<byte[]> GetBookCover(int Id);

        Task<string> GetStoresID(int Id);

        Task<int> ReservationOutOfTime(int Id);

        Task<int> RenewReservationLoan(int Id, string userName);
    }
}
