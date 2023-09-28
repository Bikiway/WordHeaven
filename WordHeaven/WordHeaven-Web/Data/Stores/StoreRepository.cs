using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data.Stores
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        private readonly DataContext _context;
        public StoreRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Store> GetAllWithUsers()
        {
            return _context.Stores.Include(s => s.user);
        }

        public IEnumerable<SelectListItem> GetComboStoresNames()
        {
            var list = _context.Stores.Select(p => new SelectListItem
            {
                Text = p.FullLocation,
                Value = p.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Location)",
                Value = "0"
            });

            return list;
        }
    }
}
