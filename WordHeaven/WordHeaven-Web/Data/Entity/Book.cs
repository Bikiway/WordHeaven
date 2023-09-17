using System.ComponentModel.DataAnnotations;

namespace WordHeaven_Web.Data.Entity
{
    public class Book : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [Display(Name = "Cover")]
        public byte[] BookCover { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        [Display(Name = "Publish Date")]
        public int YearOfPublication { get; set; }

        public string Abstract { get; set; }

        public string Language { get; set; }

        public string Category { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Value { get; set; }

        public User user { get; set; }

    }
}
