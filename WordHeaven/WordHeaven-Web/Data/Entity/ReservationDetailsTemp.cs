using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Data.Entity
{
    public class ReservationDetailsTemp : IEntity
    {
        public int Id { get; set; }

        public Store StoreName { get; set; }

        public string ClientFirstName { get; set; }

        public string ClientLastName { get; set; }

        public Book book { get; set; }

        public byte[] BookCover { get; set; }

        
        [Required]
        [DataType(DataType.EmailAddress)]
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


        [Display(Name = "Is Booked")]
        public bool IsBooked { get; set; }


        public User user { get; set; }
    }
}
