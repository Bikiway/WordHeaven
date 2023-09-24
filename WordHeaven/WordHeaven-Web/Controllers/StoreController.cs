using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Data.Stores;
using WordHeaven_Web.Helpers;

namespace WordHeaven_Web.Controllers
{
    public class StoreController : Controller
    {

        private readonly IStoreRepository _storesRepository;

        public StoreController(IStoreRepository storesRepository)
        {
            _storesRepository = storesRepository;
        }

        // GET: Store/Index
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? page)
        {
            try
            {
                int pageNumber = page ?? 1; // Página padrão é 1
                int pageSize = 10; // Número de itens por página
                var store = _storesRepository.GetAll().OrderBy(b => b.Id);
                int totalStores = store.Count();

                if (totalStores > 0)
                {
                    // Calcula o total de páginas e ajusta para não ultrapassar o limite
                    int totalPages = (int)Math.Ceiling((double)totalStores / pageSize);
                    pageNumber = Math.Max(1, Math.Min(totalPages, pageNumber));

                    // Pula e pega as stores correspondentes à página atual
                    var pagedStore = store.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    ViewBag.TotalPages = totalPages;
                    ViewBag.CurrentPage = pageNumber;

                    return View(pagedStore);
                }

                return View(null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Store/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Store/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Store model)
        {
            try
            {
                if (ModelState.IsValid)
                    await _storesRepository.CreateAsync(model);
                else
                    return View(model);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: Store/Edit/??
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("StoreNotFound");
                }

                var store = await _storesRepository.GetByIdAsync(id.Value);

                if (store == null)
                {
                    return new NotFoundViewResult("StoreNotFound");
                }

                return View(store);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Store/Edit/??
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Store model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _storesRepository.UpdateAsync(model);

                    response.Wait();

                    if (response.IsCompleted)
                        this.ViewBag.Message = "Store Data has been updated!";
                    else
                        ModelState.AddModelError(string.Empty, response.Exception.Message.ToString());
                }

                return View(model);

            }
            catch (Exception)
            {

            }

            return View(model);
        }

        // GET: Store/Delete/??
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("StoreNotFound");
                }

                var store = await _storesRepository.GetByIdAsync(id.Value);

                if (store == null)
                {
                    return new NotFoundViewResult("StoreNotFound");
                }

                return View(store);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Store/Delete/??
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storeToDelete = await _storesRepository.GetByIdAsync(id);

            if (storeToDelete != null)
            {

                await _storesRepository.DeleteAsync(storeToDelete);
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "The Store couldn't be deleted.");
            return RedirectToAction(nameof(Index));
        }

        // GET: Store/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("StoreNotFound");
                }

                var store = await _storesRepository.GetByIdAsync(id.Value);

                if (store == null)
                {
                    return new NotFoundViewResult("StoreNotFound");
                }

                return View(store);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult StoreNotFound()
        {
            return View();
        }


    }
}
