using System;
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

        Task ModifyStatusReservation(AlterStatusReservationViewModel model);

        Task<Reservation> GetReservationId(int Id);


        //Helpers

        Task<byte[]> GetBookCover(int Id);

        Task<string> GetStoresID(int Id);

        Task<int> ReservationOutOfTime(int Id, DateTime extra);

        Task<bool> RenewReservationLoan(int Id);

        Task<bool> ClientDidntReturnBook(int Id);

        Task<DateTime> GetReminderDate(int Id);

        Task<bool> TaxesPayedByClient(int Id);

        Task<bool> IsBookReturned (int Id);

        Task<bool> ClientReturnedTheBook (int Id);

        Task<string> GetStoresFromReservation(int Id);
    }
}
