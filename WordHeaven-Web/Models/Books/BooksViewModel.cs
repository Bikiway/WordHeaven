using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Models.Books
{
    public class BooksViewModel : Livro
    {
        [Display(Name = "Profile Image")]
        public IFormFile ImageProfileUser { get; set; }

    }
}
