using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Models.Users
{
    public class ChangePasswordViewModel
    {

        [Required]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}