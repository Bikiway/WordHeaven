using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Models.Users
{
    public class HelpViewModel
    {

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "ProblemDescription")]
        public string ProblemDescription { get; set; }

        [Required]
        [Display(Name = "Severity")]
        public string Severity { get; set; }


    }
}
