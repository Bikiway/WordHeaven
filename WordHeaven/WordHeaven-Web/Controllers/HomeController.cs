using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;

namespace WordHeaven_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Home()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Management()
        {
            return View();
        }

    }
}