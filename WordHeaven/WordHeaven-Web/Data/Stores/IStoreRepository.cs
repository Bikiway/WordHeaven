using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data.Stores
{
    public interface IStoreRepository : IGenericRepository<Store>
    {
        public IQueryable<Store> GetAllWithUsers();

        IEnumerable<SelectListItem> GetComboStoresNames();
    }
}
