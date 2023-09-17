using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data.Books
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(DataContext context) : base(context)
        {

        }
    }
}
