using System.ComponentModel.DataAnnotations;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Models.Employee
{
    public class EmployeesViewModel : Employees
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }
    }
}
