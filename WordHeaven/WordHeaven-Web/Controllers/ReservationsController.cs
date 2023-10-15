using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WordHeaven_Web.Data.Books;
using WordHeaven_Web.Data.Reservations;
using WordHeaven_Web.Data.Stores;
using WordHeaven_Web.Helpers;
using WordHeaven_Web.Models.Reservation;
using WordHeaven_Web.Models.Users;

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

        public ReservationsController(IReservationRepository reservationRepository, IStoreRepository storeRepository, IBookRepository bookRepository, IUserHelper userHelper, IEmailHelper emailHelper, IConfiguration configuration)
        {
            _reservationRepository = reservationRepository;
            _storeRepository = storeRepository;
            _bookRepository = bookRepository;
            _userHelper = userHelper;
            _configuration = configuration;
            _emailHelper = emailHelper;
        }


        public async Task<ActionResult> Index()
        {
            var model = await _reservationRepository.GetReservationAsync(this.User.Identity.Name);
            return View(model);
        }

        public async Task<ActionResult> Create()
        {
            var model = await _reservationRepository.GetReservationTempAsync(this.User.Identity.Name);
            return View(model);
        }


        public IActionResult MakeReservation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MakeReservation(AddReservationViewModel model, int Id)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                var image = await _reservationRepository.GetBookCover(Id);

                if (user == null)
                {
                    var arvm = new AddReservationViewModel
                    {
                        Stores = _storeRepository.GetComboStoresNames(),
                        Books = _bookRepository.GetComboBooks(),
                        BookCoverId = image,
                        ClientFirstName = model.ClientFirstName,
                        ClientLastName = model.ClientLastName,
                        UserName = model.UserName,
                        BookReturned = model.BookReturned,
                        LoanedBook = model.LoanedBook,
                        IsBooked = model.IsBooked,
                    };

                    Responses respond = _emailHelper.SendEmail(model.UserName, "Email Confirmation WordHeaven",
                       $"<h1>Reservation Confirmed</h1>" +
                       $"<h2>Thank you for choosing WordHeaven, {model.ClientFirstName} {model.ClientLastName}</h2>" +
                       $"</br>" +
                       $"<h3>Client Information</h3>" +
                       $"</br>" +
                       $"</br>" +
                       $"Client Full Name: {model.ClientFirstName} {model.ClientLastName}" +
                       $"Book: {model.Books}" +
                       $"Time Span: {model.LoanedBook} to {model.BookReturned}" +
                       $"</br>" +
                       $"</br>" +
                       $"<p>We hope for you to enjoy your reading, but don't forget to return the book because others might want to experience your journey too.</p>" +
                       $"</br>" +
                       $"</br>" +
                       $"<p>Best regards</p>" +
                       $"</br>" +
                       $"<p>Tatiane Avellar</p>");

                    if (respond.IsSuccess)
                    {
                        ViewBag.Message = "The email has been sent.";
                    }

                    await _reservationRepository.GetReservationTempAsync(model.UserName);
                    return View(arvm);
                }
            }

            return View(model);
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

            if (reserve == null)
            {
                return NotFound();
            }

            var model = new ReservationCompletedViewModel
            {
                Id = Id.Value,
            };

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

            var model = new AlterStatusReservationViewModel
            {
                Id = reservation.Id,
                StoreName = reservation.StoreName.Name,
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
        public async Task<IActionResult> ModifyReservation(AlterStatusReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _reservationRepository.ModifyStatusReservation(model);
            }

            return View(new List<AlterStatusReservationViewModel> { model });
        }


        public async Task SendWarningEmail(int Id)
        {
            var reservationId = await _reservationRepository.GetByIdAsync(Id);
            var limit = await _reservationRepository.LoanTimeLimit(reservationId.Id);

            if (limit > 0)
            {
                var user = await _userHelper.GetUserByEmailAsync(reservationId.UserName);

                string myToken = await _userHelper.GenerateConfirmEmailTokenAsync(user);
                string tokenLink = Url.Action("GetClientRenewal", "Reservations", new
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
                       $"<p>Tatiane Avellar</p>");

                if (respond.IsSuccess)
                {
                    reservationId.WarningEmailSent = true;
                }
            }
            reservationId.WarningEmailSent = false;
        }


        public async Task<IActionResult> ConfirmRenewal(string userId, string token, int Id)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var reserveId = await _reservationRepository.GetByIdAsync(Id);
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
