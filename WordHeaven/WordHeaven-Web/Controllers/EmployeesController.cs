using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WordHeaven_Web.Data.Employees;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Helpers;
using WordHeaven_Web.Models.Employees;

namespace WordHeaven_Web.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IEmployeesRepository _employeeRepository;
        private readonly IWebHostEnvironment _environment;


        public EmployeesController(IEmployeesRepository employeeRepository, IUserHelper userHelper, IWebHostEnvironment environment)
        {
            _environment = environment;
            _userHelper = userHelper;
            _employeeRepository = employeeRepository;
        }

        // GET: Employees/Index
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? page)
        {
            try
            {
                int pageNumber = page ?? 1; // Página padrão é 1
                int pageSize = 10; // Número de itens por página
                var employees = _employeeRepository.GetAll().OrderBy(b => b.Id).Include(p => p.Store);
                int totalEmployees = employees.Count();

                if (totalEmployees > 0)
                {
                    // Calcula o total de páginas e ajusta para não ultrapassar o limite
                    int totalPages = (int)Math.Ceiling((double)totalEmployees / pageSize);
                    pageNumber = Math.Max(1, Math.Min(totalPages, pageNumber));

                    // Pula e pega os employees correspondentes à página atual
                    var pagedEmployees = employees.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    ViewBag.TotalPages = totalPages;
                    ViewBag.CurrentPage = pageNumber;

                    return View(pagedEmployees);
                }

                return View(null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Employees/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new CreateNewEmployeeViewModel
            {
                Stores = _employeeRepository.GetComboStores()
            };
            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNewEmployeeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    byte[] imagemBytes = null;
                    var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    model.user = user;

                    if (model.EmployeeImageFile != null && model.EmployeeImageFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.EmployeeImageFile.CopyToAsync(memoryStream);

                            // If to controll the file size less (2 MB)
                            if (memoryStream.Length < 2097152)
                            {
                                model.Image = memoryStream.ToArray();
                            }
                            else
                            {
                                ModelState.AddModelError("EmployeeImageFile", "The file is too large. Max size is 2 MB");
                                model.Stores = _employeeRepository.GetComboStores();
                                return View(model);
                            }
                        }
                    }
                    else
                    {
                        string completePath = Path.Combine(_environment.WebRootPath, "assets", "images", "profile-default-image.jpg");
                        imagemBytes = System.IO.File.ReadAllBytes(completePath);
                        model.Image = imagemBytes.ToArray();
                    }

                    var verifyUser = await _userHelper.GetUserByEmailAsync(model.Email);

                    if (verifyUser == null)
                    {
                        var employeeUser = new User
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            UserName = model.Email,
                            Address = model.Address,
                            PostalCode = model.PostalCode,
                            Location = model.Location,
                            PhoneNumber = model.Phone,
                            PictureSource = model.Image
                        };

                        var result = await _userHelper.AddUserAsync(employeeUser, "Aa1234567890");
                        await _userHelper.AddUserToRoleAsync(employeeUser, "Employee");
                        var token = await _userHelper.GenerateConfirmEmailTokenAsync(employeeUser);
                        await _userHelper.EmailConfirmAsync(employeeUser, token);

                        if (result != IdentityResult.Success)
                        {
                            ModelState.AddModelError(string.Empty, "The Employee couldn´t be created.");
                            model.Stores = _employeeRepository.GetComboStores();
                            return View(model);
                        }
                        else
                        {
                            model.EmployeeUserId = employeeUser.Id;
                            await _employeeRepository.CreateAsync(model);
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    ModelState.AddModelError(string.Empty, "The Employee couldn´t be created. Email already registed.");
                    model.Stores = _employeeRepository.GetComboStores();
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(model);
            }
        }

        // GET: Employees/Edit/??
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("EmployeeNotFound");
                }

                var employee = await _employeeRepository.GetByIdAsync(id.Value);

                if (employee == null)
                {
                    return new NotFoundViewResult("EmployeeNotFound");
                }

                var model = new EditEmployeeViewModel();

                model.JobTitle = employee.JobTitle;
                model.Stores = _employeeRepository.GetComboStores();
                model.StoreId = employee.StoreId;

                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Employees/Edit/??
        [HttpPost]
        public async Task<IActionResult> Edit(EditEmployeeViewModel model)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(model.Id);
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                employee.JobTitle = model.JobTitle;
                employee.StoreId = model.StoreId;
                employee.user = user;

                var response = _employeeRepository.UpdateAsync(employee);

                response.Wait();

                if (response.IsCompleted)
                {
                    model.Stores = _employeeRepository.GetComboStores();
                    this.ViewBag.Message = "Employee Data has been updated!";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Exception.Message.ToString());
                    model.Stores = _employeeRepository.GetComboStores();
                }

                return View(model);

            }
            catch (Exception)
            {

            }

            return View(model);
        }

        // GET: Employees/Delete/??
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("EmployeeNotFound");
                }

                var employee = await _employeeRepository.GetByIdAsync(id.Value);

                if (employee == null)
                {
                    return new NotFoundViewResult("EmployeeNotFound");
                }

                return View(employee);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Employees/Delete/???
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeToDelete = await _employeeRepository.GetByIdAsync(id);

            if (employeeToDelete != null)
            {
                var userToDelete = await _userHelper.GetUserByIdAsync(employeeToDelete.EmployeeUserId);

                if (userToDelete != null)
                {
                    var result = await _userHelper.DeleteUserAsync(userToDelete);

                    if (result.Succeeded)
                    {
                        await _employeeRepository.DeleteAsync(employeeToDelete);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // Mensagem de erro
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "The Employee couldn't be deleted.");
            return RedirectToAction(nameof(Index));
        }

        //GET: Employees/Details/???
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("EmployeeNotFound");
                }

                var employee = await _employeeRepository.GetByIdAsync(id.Value);

                if (employee == null)
                {
                    return new NotFoundViewResult("EmployeeNotFound");
                }

                return View(employee);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult EmployeeNotFound()
        {
            return View();
        }

    }
}
