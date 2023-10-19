using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WordHeaven_Web.Data;
using WordHeaven_Web.Data.Books;
using WordHeaven_Web.Data.Reservations;
using WordHeaven_Web.Data.Stores;
using WordHeaven_Web.Helpers;
using WordHeaven_Web.Models.Reservation;

namespace WordHeaven_Web.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IEmailHelper _emailHelper;
        private readonly IConfiguration _configuration;
        private readonly IUserHelper _userHelper;
        private readonly DataContext _dataContext;

        public ReservationsController(IReservationRepository reservationRepository, IStoreRepository storeRepository, IBookRepository bookRepository, IUserHelper userHelper, IEmailHelper emailHelper, IConfiguration configuration, DataContext dataContext)
        {
            _reservationRepository = reservationRepository;
            _storeRepository = storeRepository;
            _bookRepository = bookRepository;
            _userHelper = userHelper;
            _configuration = configuration;
            _emailHelper = emailHelper;
            _dataContext = dataContext;
        }


        public async Task<ActionResult> Index(int Id)
        {
            var model = await _reservationRepository.GetReservationAsync(this.User.Identity.Name);
            var models = await _reservationRepository.GetReservationAsync(this.User.Identity.Name);

            var reservation = await _reservationRepository.IsBookReturned(Id);

            if (reservation == true)
            {
                return View(model);
            }

            else
            {
                return View(models);
            }


        }

        public async Task<ActionResult> Create()
        {
            var model = await _reservationRepository.GetReservationTempAsync(this.User.Identity.Name);
            return View(model);
        }

        public async Task<IActionResult> GetStatistics(int Id)
        {
            var getClients = await _reservationRepository.GetReservationAsync(this.User.Identity.Name);

            var getReserves = _dataContext.Reservations?.Count();
            var totalBooks = _dataContext.Books?.Count();
            var totalOfEmployees = _dataContext.Employees?.Count();
            var clientsCount = getClients?.Count();
            var IsBooked = _dataContext.Reservations?.Where(p => p.Id == Id && p.IsBooked == true).Count();



            // String with the statistics
            var statisticsHtml = $"<p style='font-size: 20px;'><b>Reservations:</b> {getReserves ?? 0}</p>" +
                       $"<p style='font-size: 20px;'><b>Clients:</b> {clientsCount ?? 0}</p>" +
                       $"<p style='font-size: 20px;'><b>Total Books:</b> {totalBooks ?? 0}</p>" +
                       $"<p style='font-size: 20px;'><b>Employees:</b> {totalOfEmployees ?? 0}</p>" +
                       $"<p style='font-size: 20px;'><b>Books Reserved:</b> {IsBooked ?? 0}</p>";

            // Return the styled statistics as HTML
            return Content(statisticsHtml, "text/html");
        }


        [HttpPost]
        public async Task<IActionResult> MakeReservation(AddReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _reservationRepository.AddItemToReservationAsync(model, model.UserName);

                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    string myToken = await _userHelper.GenerateConfirmEmailTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Reservations", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    Responses respond = _emailHelper.SendEmail(model.UserName, "Email Confirmation WordHeaven",
                           $"<h1>Reservation Confirmed</h1>" +
                           $"<h2>Thank you for choosing WordHeaven, {model.ClientFirstName} {model.ClientLastName}</h2>" +
                           $"</br>" +
                           $"<h3>Client Information</h3>" +
                           $"</br>" +
                           $"</br>" +
                           $"Client Name: {model.ClientFirstName} {model.ClientLastName}" +
                           $"</br>" +
                           $"Book: {model.BookId}" +
                           $"Time Span: {model.LoanedBook} to {model.BookReturned}" +
                           $"</br>" +
                           $"</br>" +
                           $"<p>We hope for you to enjoy your reading, but don't forget to return the book because others might want to experience your journey too.</p>" +
                           $"</br>" +
                           $"To confirm this reservation, please click in this <a href= \"{tokenLink}\"> link </a>" +
                           $"</br>" +
                           $"</br>" +
                           $"<p>Best regards</p>" +
                           $"</br>" +
                           $"<p>Tatiane Avellar e Mariana Oliveira</p>");

                    if (respond.IsSuccess)
                    {
                        ViewBag.Message = "The email has been sent.";
                    }

                    await _reservationRepository.GetReservationTempAsync(model.UserName);
                    return RedirectToAction("create");
                }


            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.EmailConfirmAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }


        public IActionResult MakeReservation()
        {
            var arvm = new AddReservationViewModel
            {
                Stores = _storeRepository.GetComboStoresNames(),
                Books = _bookRepository.GetComboBooks(),
                ClientFirstName = "",
                ClientLastName = "",
                UserName = "",
                BookReturned = DateTime.MinValue,
                LoanedBook = DateTime.MinValue,
                IsBooked = true,
            };

            return View(arvm);
        }


        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            await _reservationRepository.DeleteTempAsync(Id.Value);
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> ConfirmReservation()
        {
            var response = await _reservationRepository.ConfirmReservationAsync(this.User.Identity.Name);

            if (response)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Create");
        }

        public async Task<IActionResult> SaveReservation(int? Id)
        {
            if (Id == null)
            { return NotFound(); }

            var reserve = await _reservationRepository.GetByIdAsync(Id.Value);
            DateTime limit = await _reservationRepository.GetReminderDate(reserve.Id);

            if (reserve == null)
            {
                return NotFound();
            }

            var model = new ReservationCompletedViewModel
            {
                Id = Id.Value,
                emailSent = limit,
            };

            await SendWarningEmail(reserve.Id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveReservation(ReservationCompletedViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _reservationRepository.ReservationCompleted(model);
                return RedirectToAction("Index");
            }

            return View();
        }


        public async Task<IActionResult> ModifyReservation(int? Id)
        {
            if (Id == null)
            { return NotFound(); }

            var reservation = await _reservationRepository.GetByIdAsync(Id.Value);

            if (reservation == null)
            { return NotFound(); }

            var stores = _reservationRepository.GetStoresFromReservation(Id.Value);

            var model = new AlterStatusReservationViewModel
            {
                Id = reservation.Id,
                StoreName = stores.Result,
                ClientUserName = reservation.UserName,
                BookAsReturned = reservation.BookReturnedByClient,
                DidntReturnTheBook = reservation.ClientDidntReturnTheBook,
                RenewTime = reservation.RenewBookLoan,
                ApplyTaxes = reservation.PayTaxesLoan,
                ClientPayedTaxes = reservation.PayedTaxesLoan,
            };

            return View(new List<AlterStatusReservationViewModel> { model });
        }

        [HttpPost]
        public async Task<IActionResult> ModifyReservation(AlterStatusReservationViewModel model, int Id)
        {
            if (ModelState.IsValid)
            {
                await _reservationRepository.ModifyStatusReservation(model);
            }

            return View(new List<AlterStatusReservationViewModel> { model });
        }


        public async Task<IActionResult> SendWarningEmail(int Id)
        {
            var reservationId = await _reservationRepository.GetReservationId(Id);
            var limit = await _reservationRepository.GetReminderDate(reservationId.Id);

            if (limit.Date != null)
            {
                var user = await _userHelper.GetUserByEmailAsync(reservationId.UserName);

                string myToken = await _userHelper.GenerateConfirmEmailTokenAsync(user);
                string tokenLink = Url.Action("ConfirmRenewal", "Reservations", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Responses respond = _emailHelper.SendEmail(reservationId.UserName, "Warning Email - WordHeaven",
                       $"<h1>Reservation: {reservationId.BookName}</h1>" +
                       $"</br>" +
                       $"<h3>Thank you for choosing WordHeaven, but unfortunately your reservation is coming to an end.</h3>" +
                       $"</br>" +
                       $"<p>{reservationId.ClientFirstName} {reservationId.ClientLastName} don't forget you have three more days to read your amazing book. </p>" +
                       $"</br>" +
                       $"If you wish to renew your reservation for more twenty days, please click in this link: <a href = \"{tokenLink}\"><b>Confirm your renewal</b></a>" +
                       $"</br>" +
                       $"<p>We hope for you to enjoy your rest of your reading.</p>" +
                       $"</br>" +
                       $"</br>" +
                       $"<p>Best regards</p>" +
                       $"</br>" +
                       $"<p>Tatiane Avellar e Mariana Oliveira</p>");

                if (respond.IsSuccess)
                {
                    reservationId.WarningEmailSent = true;
                }
            }
            return View();
        }


        public async Task<IActionResult> SendResponsabilizationEmail(int Id)
        {
            var reservationId = await _reservationRepository.GetReservationId(Id);

            var limit = await _reservationRepository.ClientDidntReturnBook(reservationId.Id);
            var taxes = await _reservationRepository.TaxesPayedByClient(reservationId.Id);

            if (limit == true && taxes == false)
            {
                var user = await _userHelper.GetUserByEmailAsync(reservationId.UserName);

                string myToken = await _userHelper.GenerateConfirmEmailTokenAsync(user);
                string tokenLink = Url.Action("ResponsabilizationForClient", "Reservations", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Responses respond = _emailHelper.SendEmail(reservationId.UserName, "Warning Email - WordHeaven",
                       $"<h1>Reservation: {reservationId.BookName}</h1>" +
                       $"</br>" +
                       $"<h3>Thank you for choosing WordHeaven, but unfortunately your reservation is coming to an end.</h3>" +
                       $"</br>" +
                       $"<p>{reservationId.ClientFirstName} {reservationId.ClientLastName} don't forget as of today, you are oblige to pay 1€ per day you didn't return your book. </p>" +
                       $"</br>" +
                       $"If you wish to pay your fees, please click in this link: <a href = \"{tokenLink}\"><b>Pay Fees</b></a>" +
                       $"</br>" +
                       $"<p>We hope for you to don't give up on reading books and we are always here to assist in any question you might have.</p>" +
                       $"</br>" +
                       $"</br>" +
                       $"<p>Best regards</p>" +
                       $"</br>" +
                       $"<p>Tatiane Avellar e Mariana Oliveira</p>");

                if (respond.IsSuccess)
                {
                    reservationId.WarningEmailSent = true;
                }
            }
            return View();
        }

        public async Task<IActionResult> ResponsabilizationForClient(string userId, string token, int Id)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var reserveId = await _reservationRepository.GetReservationId(Id);
            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null || reserveId == null)
            {
                return NotFound();
            }

            var result = await _userHelper.EmailConfirmAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            await _reservationRepository.RenewReservationLoan(reserveId.Id);

            return View();
        }

        public async Task<IActionResult> SendRenewalEmail(int Id)
        {
            var reservationId = await _reservationRepository.GetReservationId(Id);
            var limit = await _reservationRepository.RenewReservationLoan(reservationId.Id);

            if (limit == true)
            {
                var user = await _userHelper.GetUserByEmailAsync(reservationId.UserName);

                string myToken = await _userHelper.GenerateConfirmEmailTokenAsync(user);
                string tokenLink = Url.Action("GetClientsRenewal", "Reservations", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Responses respond = _emailHelper.SendEmail(reservationId.UserName, "Warning Email - WordHeaven",
                       $"<h1>Reservation: {reservationId.BookName}</h1>" +
                       $"</br>" +
                       $"<h3>Congrats, your book awaits for you for another 30 days.</h3>" +
                       $"</br>" +
                       $"<p>{reservationId.ClientFirstName} {reservationId.ClientLastName} </p>" +
                       $"</br>" +
                       $"Please click in this link: <a href = \"{tokenLink}\"><b>Confirm your renewal</b></a>" +
                       $"</br>" +
                       $"<p>We hope for you to enjoy your reading.</p>" +
                       $"</br>" +
                       $"</br>" +
                       $"<p>Best regards</p>" +
                       $"</br>" +
                       $"<p>Tatiane Avellar e Mariana Oliveira</p>");

                if (respond.IsSuccess)
                {
                    reservationId.WarningEmailSent = true;
                }
            }
            return View();
        }


        public async Task<IActionResult> ConfirmRenewal(string userId, string token, int Id)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var reserveId = await _reservationRepository.GetReservationId(Id);
            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null || reserveId == null)
            {
                return NotFound();
            }

            var result = await _userHelper.EmailConfirmAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            await _reservationRepository.RenewReservationLoan(reserveId.Id);

            return View();
        }

        public async Task<IActionResult> GetClientsRenewal(string userId, string token, int Id)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var reserveId = await _reservationRepository.GetReservationId(Id);
            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null || reserveId == null)
            {
                return NotFound();
            }

            var result = await _userHelper.EmailConfirmAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }

            await _reservationRepository.RenewReservationLoan(reserveId.Id);

            return View();
        }

    }
}
