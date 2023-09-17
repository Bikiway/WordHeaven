using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Livro> Livros { get; set; }
        public DbSet<Employees> Employee { get; set; }

        public DbSet<Stores> Store { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}