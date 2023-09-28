using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationDetails> ReservationsDetail { get; set; }
        public DbSet<ReservationDetailsTemp> ReservationsDetailTemp { get; set; }


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
        .HasOne(employee => employee.Store)
        .WithMany(store => store.Employees) // Aqui, você está configurando a propriedade de navegação para funcionários
        .HasForeignKey(employee => employee.StoreId)
        .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}