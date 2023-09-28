using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Models.Reservation
{
    public class AddReservationViewModel
    {
        public int storeId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ClientFirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string ClientLastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }


        [Display(Name = "Book")]
        [Range(1, int.MaxValue, ErrorMessage = "You must choose a book.")]
        public int BookId { get; set; }

        public byte[] BookCoverId { get; set; }


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

        public IEnumerable<SelectListItem> Books { get; set; }

        public IEnumerable<SelectListItem> Stores { get; set; }

    }
}
