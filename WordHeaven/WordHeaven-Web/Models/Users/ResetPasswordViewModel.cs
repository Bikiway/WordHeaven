using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Models.Users
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string Confirm { get; set; }

        [Required]
        public string Token { get; set; }
    }
}