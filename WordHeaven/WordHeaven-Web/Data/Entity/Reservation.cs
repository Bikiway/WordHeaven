using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WordHeaven_Web.Data.Entity
{
    public class Reservation : IEntity
    {
        public int Id { get; set; }

        public Store StoreName { get; set; }

        public string ClientFirstName { get; set; }

        public string ClientLastName { get; set; }

        public string FullClientName => $"{ClientFirstName} {ClientLastName}";
        [Display(Name = "Email")]
        [EmailAddress]
        public string UserName { get; set; }


        [Display(Name = "Book")]
        public string BookName { get; set; }


        [Display(Name = "Cover")]
        public byte BookCover { get; set; }


        [Display(Name = "Delivery Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime LoanedBook { get; set; }



        [Display(Name = "Return Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime BookReturned { get; set; }


        [Display(Name = "Time Limit")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime LoanTimeLimit { get; set; }


        public bool WarningEmailSent { get; set; }

        [Display(Name = "Is Booked")]
        public bool IsBooked { get; set; }


        [Display(Name = "Book Returned")]
        public bool BookReturnedByClient { get; set; }


        [Display(Name = "Not Returned")]
        public bool ClientDidntReturnTheBook { get; set; }


        [Display(Name = "Taxes")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PayTaxesLoan { get; set; }


        [Display(Name = "Payed Loan")]
        public bool PayedTaxesLoan { get; set; }


        [Display(Name = "Renew Loan")]
        public bool RenewBookLoan { get; set; }


        public IEnumerable<ReservationDetails> Items { get; set; }


        [Display(Name = "Requested Books")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Request => Items == null ? 0 : Items.Sum(b => b.Request);



        [Required]
        public User user { get; set; }

    }
}
