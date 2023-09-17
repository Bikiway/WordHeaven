using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Models.Books;

namespace WordHeaven_Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Book> Books { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}