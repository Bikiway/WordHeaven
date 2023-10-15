using System;

namespace WordHeaven_Web.Models.Reservation
{
    public class AlterStatusReservationViewModel
    {
        public int Id { get; set; }

        public string StoreName { get; set; }

        public string ClientUserName { get; set; }

        public bool BookAsReturned { get; set; }

        public bool DidntReturnTheBook { get; set; }

        public int ApplyTaxes { get; set; }

        public bool RenewTime { get; set; }

        public bool ClientPayedTaxes { get; set; }
    }
}
