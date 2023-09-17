using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Models.Books
{
    public class CreateBookViewModel : Book
    {     
        public IFormFile? BookCoverPicture { get; set; }

    }
}
