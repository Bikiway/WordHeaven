using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data.Books
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        public IQueryable<Book> GetAllWithUsers();

        IEnumerable<SelectListItem> GetComboBooks();

        IEnumerable<SelectListItem> GetComboCoverBooks();
    }
}
