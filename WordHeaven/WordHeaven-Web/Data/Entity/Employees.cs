using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Data.Entity
{
    public class Employees : IEntity
    {
        public int Id { get; set; }


        public string FirstName { get; set; }


        public string LastName { get; set; }


        [MaxLength(3, ErrorMessage = "The field {0} only can contain {1} characters length")]
        public string Age { get; set; }


        [MaxLength(20, ErrorMessage = "The field {0} only can contain {1} characters length")]
        public string PhoneNumber { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }


        public string JobTitle { get; set; }


        //comboBox
        public Stores stores { get; set; }

        public int storeId { get; set; }

        //user
        public User user { get; set; }
    }
}
