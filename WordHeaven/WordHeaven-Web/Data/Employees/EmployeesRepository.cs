using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Data.Employees
{
    public class EmployeesRepository : GenericRepository<Employee>, IEmployeesRepository
    {
        private readonly DataContext _context;

        public EmployeesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboStores()
        {
            var list = _context.Stores.Select(p => new SelectListItem
            {

                Text = p.Name,
                Value = p.Id.ToString()

            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a store....)",
                Value = "0"
            });

            return list;
        }
    }
}
