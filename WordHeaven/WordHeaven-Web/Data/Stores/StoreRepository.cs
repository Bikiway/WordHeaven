using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data.Stores
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        public StoreRepository(DataContext context) : base(context)
        {
        }
    }
}
