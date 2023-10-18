using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data.Books
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly DataContext _context;
        public BookRepository(DataContext context) : base(context)
        {
            _context = context;   
        }

        public IQueryable<Book> GetAllWithUsers()
        {
            return _context.Books.Include(b => b.user);
        }

        public IEnumerable<SelectListItem> GetComboBooks()
        {
            var list = _context.Books.Select(b => new SelectListItem
            {
                Text = b.Title,
                Value = b.Id.ToString(),
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Book)",
                Value = "0",
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboCoverBooks()
        {
            var list = _context.Books.Select(b => new SelectListItem
            {
                Text = b.BookCover.ToString(),
                Value = b.Id.ToString(),
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Book Cover)",
                Value = "0",
            });

            return list;
        }

        public IQueryable GetBooks()
        {
            return _context.Books.Include(s => s.BookCover);
        }
    }
}
