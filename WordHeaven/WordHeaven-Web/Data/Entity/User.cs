﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Data.Entity
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} cannot have more then {1} characters.")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Required]
        public string Location { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public override string? PhoneNumber { get; set; }

        [Display(Name = "Image")]
        public byte[] PictureSource { get; set; }

        [Display(Name = "Full Name")]
        public string? FullName => $"{FirstName} {LastName}";
    }
}