using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Data.Entity
{
    public class Store : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Store")]
        public string Name { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} cannot have more then {1} characters.")]
        public string Address { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
