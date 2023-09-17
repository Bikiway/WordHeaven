using Microsoft.AspNetCore.Http;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Models.Books
{
    public class EditBookViewModel : Book
    {
        public IFormFile? BookCoverPicture { get; set; }

    }
}
