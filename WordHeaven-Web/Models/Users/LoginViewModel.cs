using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Models.Users
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string? UserName { get; set; }

        [Required]
        [MinLength(9)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; }
    }
}