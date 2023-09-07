using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Helpers;
using WordHeaven_Web.Models.Users;
using WordHeaven_Web.Models;
using WordHeaven_Web.Data;
using System.Linq;

namespace WordHeaven_Web.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IConfiguration _configuration;
        private readonly IEmployeesRepository _employeeRepository;
        private readonly IConverterHelper _converterHelper;


        public EmployeesController(IConverterHelper converterHelper, IEmailHelper emailHelper, IEmployeesRepository employeeRepository, IUserHelper userHelper, IConfiguration configuration)
        {
            _configuration = configuration;
            _converterHelper = converterHelper;
            _emailHelper = emailHelper;
            _employeeRepository = employeeRepository;
            _userHelper = userHelper;
        }

        // GET: EmployeesController
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(_employeeRepository.GetAllWithUser().OrderBy(o => o.Id));
        }

        // GET: EmployeesController/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var employees = await _employeeRepository.GetByIdAsync(id.Value);

            if (employees == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            return View(employees);
        }

        // GET: EmployeesController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);

                if (user == null)
                {
                    // var IdEmploy = await _userHelper.GetUserByIdAsync(user.Id);

                    user = new User
                    {
                        //Id = change.UserName,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.UserName,
                        UserName = model.UserName,
                        Age = model.Age,
                        PhoneNumber = model.PhoneNumber,
                        JobTitle = model.JobTitle,
                    };

                    //Caso não consiga criar um login novo

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }

                    //Image
                    //var path = string.Empty;

                    //if (model.ImageProfile != null && model.ImageProfile.Length > 0)
                    //{
                    //    path = await _imageHelper.UploadImageAsync(model.ImageProfile, "employees");
                    //}

                    //Token Email
                    string myToken = await _userHelper.GenerateConfirmEmailTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Employees", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);


                    Responses response = _emailHelper.SendEmail(model.UserName, "Email Confirmation AirFiel", $"<h1>Employee Email Confirmation</h1>" +
                        $"<h2>Welcome to our team!</h2>" +
                        $"<h3>Please click in this link</h3>" +
                        $"</br>" +
                        $"</br>" +
                        $"<a href = \"{tokenLink}\">Confirm Employee Email</a>");

                    if (response.IsSuccess)
                    {
                        ViewBag.Message = "The email has been sent.";

                        var employees = _converterHelper.ToEmployees(model, true);

                        employees.user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                        await _employeeRepository.CreateAsync(employees);

                        return View(model);
                    }



                }


                ModelState.AddModelError(string.Empty, "The user couldn't be logged.");

            }
            return View(model);
        }



        // GET: Employees/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var employees = await _employeeRepository.GetByIdAsync(id.Value);

            if (employees == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var model = _converterHelper.ToEmployeesViewModel(employees);
            return View(model);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(EmployeesViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (user != null)
            {
                try
                {
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Age = user.Age;
                    model.PhoneNumber = user.PhoneNumber;

                    //var path = model.ProfileImage;

                    //if (model.ImageProfile != null && model.ImageProfile.Length > 0)
                    //{
                    //    path = await _imageHelper.UploadImageAsync(model.ImageProfile, "employees");
                    //}

                    var employees = _converterHelper.ToEmployees(model, false);

                    employees.user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _employeeRepository.UpdateAsync(employees);


                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User updated!";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _employeeRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("EmployeeNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var employees = await _employeeRepository.GetByIdAsync(id.Value);

            if (employees == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            return View(employees);
        }


        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);
                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"])); //Algoritmo para ir buscar a key (No appsettings.json).
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //Gerar o token, usando o algoritmo que vem do security key. 256 bits. Depende do middleware.
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);
                    }
                }
            }
            return BadRequest();
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId); //Verificar se tem user
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.EmailConfirmAsync(user, token); //Vê se está tudo Okay.

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }




        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_employeeRepository == null)
            {
                return Problem("Entity set 'DataContext.employees' is null");
            }

            var employees = await _employeeRepository.GetByIdAsync(id);

            if (employees != null)
            {
                await _employeeRepository.DeleteAsync(employees);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EmployeeNotFound()
        {
            return View();
        }
    }
}
