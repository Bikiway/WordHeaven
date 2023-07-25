using System.Threading.Tasks;

namespace WordHeaven_Web.Data
{
    public class SeedDB
    {
        private readonly DataContext _context;
        public SeedDB(DataContext context)
        {
            _context = context;
        }
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await _context.SaveChangesAsync();
        }
    }
}
