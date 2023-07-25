using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        //Criar a tabela de livros
        public DbSet<Livro> livros { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
    }
}
