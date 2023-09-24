using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WordHeaven_Web.Data.Entity;

namespace WordHeaven_Web.Models.Employees
{
    public class CreateNewEmployeeViewModel : Employee
    {
        public IEnumerable<SelectListItem> Stores { get; set; }

        [Display(Name = "Image")]
        public IFormFile? EmployeeImageFile { get; set; }
    }
}
