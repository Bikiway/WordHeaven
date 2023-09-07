using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Models.Users
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}