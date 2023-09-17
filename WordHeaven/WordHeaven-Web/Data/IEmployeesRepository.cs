using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data
{
    public interface IEmployeesRepository : IGenericRepository<Employees>
    {
        public IQueryable GetAllWithUsers();

        IEnumerable<SelectListItem> GetComboEmployees();
    }
}
