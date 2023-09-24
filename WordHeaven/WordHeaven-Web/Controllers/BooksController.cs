using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WordHeaven_Web.Data;
using WordHeaven_Web.Data.Books;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Helpers;
using WordHeaven_Web.Models.Books;
using WordHeaven_Web.Models.Users;

namespace WordHeaven_Web.Controllers
{
    public class BooksController : Controller
    {

        private readonly IBookRepository _bookRepository;
        private readonly IUserHelper _userHelper;
        private readonly IWebHostEnvironment _environment;

        public BooksController(IBookRepository bookRepository, IUserHelper userHelper, IWebHostEnvironment environment)
        {
            _bookRepository = bookRepository;
            _userHelper = userHelper;
            _environment = environment;
        }

        // GET: Books/Index
        public ActionResult Index(int? page)
        {
            try
            {
                int pageNumber = page ?? 1; // Página padrão é 1
                int pageSize = 10; // Número de itens por página
                var books = _bookRepository.GetAll().OrderBy(b => b.Id);
                int totalBooks = books.Count();

                if (totalBooks > 0)
                {
                    // Calcula o total de páginas e ajusta para não ultrapassar o limite
                    int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
                    pageNumber = Math.Max(1, Math.Min(totalPages, pageNumber));

                    // Pula e pega os livros correspondentes à página atual
                    var pagedBooks = books.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    ViewBag.TotalPages = totalPages;
                    ViewBag.CurrentPage = pageNumber;

                    return View(pagedBooks);
                }

                return View(null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Books/Create
        [Authorize(Roles = "employee,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                var book = new Book
                {
                    Title = model.Title,
                    Author = model.Author,
                    Publisher = model.Publisher,
                    YearOfPublication = model.YearOfPublication,
                    Abstract = model.Abstract,
                    Language = model.Language,
                    Category = model.Category,
                    Value = model.Value,
                    user = user
                };

                if (model.BookCoverPicture != null && model.BookCoverPicture.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.BookCoverPicture.CopyToAsync(memoryStream);

                        // If to controll the file size less (2 MB)
                        if (memoryStream.Length < 2097152)
                        {
                            book.BookCover = memoryStream.ToArray();
                        }
                        else
                        {
                            this.ModelState.AddModelError("BookCoverPicture", "The file is too large. Max size is 2 MB");
                            return View(model);
                        }
                    }
                }
                else
                {
                    string completePath = Path.Combine(_environment.WebRootPath, "assets", "images", "default-book-cover.jpg");
                    byte[] imagemBytes = System.IO.File.ReadAllBytes(completePath);
                    book.BookCover = imagemBytes.ToArray();
                }

                await _bookRepository.CreateAsync(book);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Books/Edit/??
        [Authorize(Roles = "employee,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("BookNotFound");
                }

                var book = await _bookRepository.GetByIdAsync(id.Value);

                if (book == null)
                {
                    return new NotFoundViewResult("BookNotFound");
                }

                var model = new EditBookViewModel();

                model.Title = book.Title;
                model.Author = book.Author;
                model.Publisher = book.Publisher;
                model.YearOfPublication = book.YearOfPublication;
                model.Abstract = book.Abstract;
                model.Language = book.Language;
                model.Category = book.Category;
                model.Value = book.Value;

                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Books/Edit/??
        [HttpPost]
        public async Task<IActionResult> Edit(EditBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    var book = await _bookRepository.GetByIdAsync(model.Id);

                    book.Title = model.Title;
                    book.Author = model.Author;
                    book.Publisher = model.Publisher;
                    book.YearOfPublication = model.YearOfPublication;
                    book.Abstract = model.Abstract;
                    book.Language = model.Language;
                    book.Category = model.Category;
                    book.Value = model.Value;
                    book.user = user;


                    if (model.BookCoverPicture != null && model.BookCoverPicture.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.BookCoverPicture.CopyToAsync(memoryStream);

                            // If to controll the file size less (2 MB)
                            if (memoryStream.Length < 2097152)
                            {
                                book.BookCover = memoryStream.ToArray();
                            }
                            else
                            {
                                ModelState.AddModelError("BookCoverPicture", "The file is too large. Max size is 2 MB");
                            }
                        }
                    }


                    var response = _bookRepository.UpdateAsync(book);

                    response.Wait();

                    if (response.IsCompleted)
                        this.ViewBag.Message = "Your Book has been updated!";
                    else
                        ModelState.AddModelError(string.Empty, response.Exception.Message.ToString());
                    return View(model);

                }
                catch (Exception)
                {

                }
            }
            return View(model);
        }

        // GET: Books/Delete/??
        [Authorize(Roles = "employee,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("BookNotFound");
                }

                var book = await _bookRepository.GetByIdAsync(id.Value);

                if (book == null)
                {
                    return new NotFoundViewResult("BookNotFound");
                }

                return View(book);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Books/Delete/???
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            await _bookRepository.DeleteAsync(book);
            return RedirectToAction(nameof(Index));
        }

        //GET: Books/Details/???
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("BookNotFound");
                }

                var book = await _bookRepository.GetByIdAsync(id.Value);

                if (book == null)
                {
                    return new NotFoundViewResult("BookNotFound");
                }

                return View(book);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult BookNotFound()
        {
            return View();
        }
    }
}
