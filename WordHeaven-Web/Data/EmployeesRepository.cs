using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WordHeaven_Web.Data.Entity;
using System.Linq;

namespace WordHeaven_Web.Data
{
    public class EmployeesRepository : GenericRepository<Employees>, IEmployeeRepository
    {
        private readonly DataContext _context;

        public EmployeesRepository(DataContext context) : base( context )
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Employee.Include(p => p.user);
        }

        public IEnumerable<SelectListItem> GetComboEmployees()
        {
            var list = _context.Employee.Select(p => new SelectListItem
            {
                Text = p.FirstName,
                Value = p.Id.ToString(),
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select an Employee)",
                Value = "0",
            });

            return list;
        }
    }
}
