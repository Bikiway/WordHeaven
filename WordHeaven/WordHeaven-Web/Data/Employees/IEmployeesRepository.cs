using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data.Employees
{
    public interface IEmployeesRepository : IGenericRepository<Employee>
    {
        IEnumerable<SelectListItem> GetComboStores();
    }
}
