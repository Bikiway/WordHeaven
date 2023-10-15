using System;
using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Data.Entity
{
    public class ReservationDetails : IEntity
    {
        public int Id { get; set; }

        public Store StoreName { get; set; }

        [Display(Name = "Book")]
        public Book bookName { get; set; }


        [Display(Name = "Image")]
        public byte[] BookCover { get; set; }

        public string ClientFirstName { get; set; }

        public string ClientLastName { get; set; }

        public string UserName { get; set; }


        [Display(Name = "Delivery Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime LoanedBook { get; set; }



        [Display(Name = "Return Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime BookReturned { get; set; }


        [Display(Name = "Requested Books")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Request { get; set; }
    }
}
