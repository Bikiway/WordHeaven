using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Models.Users
{
    public class ChangePersonalInformationViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public IFormFile? PictureUser { get; set; }
    }
}