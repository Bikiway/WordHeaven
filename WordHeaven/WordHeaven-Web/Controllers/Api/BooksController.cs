using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WordHeaven_Web.Data.Books;

namespace WordHeaven_Web.Controllers.Api
{
    [Route("api/[Controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }


        [HttpGet]
        public IActionResult GetBook()
        {
            return Ok(_bookRepository.GetAllWithUsers());
        }
    }
}
