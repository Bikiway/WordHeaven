using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Data.Entity
{
    public class Employee : IEntity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        public Store Store { get; set; }

        public int StoreId { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} cannot have more then {1} characters.")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Image")]
        public byte[] Image { get; set; }

        [Display(Name = "Full Name")]
        public string EmployeeFullName => $"{FirstName} {LastName} ";

        public User user { get; set; }

        public string EmployeeUserId { get; set; }

    }
}
