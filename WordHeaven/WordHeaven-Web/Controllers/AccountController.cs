using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Helpers;
using WordHeaven_Web.Models.Users;

namespace WordHeaven_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly UserManager<User> _userManager;
        private readonly IEmailHelper _emailHelper;
        private readonly IWebHostEnvironment _environment;

        public AccountController(IUserHelper userHelper, UserManager<User> userManager, IEmailHelper emailHelper, IWebHostEnvironment environment)
        {
            _userHelper = userHelper;
            _userManager = userManager;
            _emailHelper = emailHelper;
            _environment = environment;
        }

        public IActionResult AccountManagement()
        {
            return View();
        }

        #region Login/Logout

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Home", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                var emailConfirmed = await _userHelper.IsEmailConfirmedAsync(user);

                if (result.Succeeded && emailConfirmed == true)
                    return RedirectToAction("Home", "Home");
                else if (emailConfirmed == false)
                {
                    this.ModelState.AddModelError(string.Empty, "Email not confirmed. Please check your email");
                    return View(model);
                }
            }

            this.ModelState.AddModelError(string.Empty, "Login failed! Please enter the correct username and password");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Home", "Home");
        }

        #endregion Login/Logout

        #region Register

        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterNewUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userHelper.GetUserByEmailAsync(model.Username);

                    string completePath = Path.Combine(_environment.WebRootPath, "assets", "images", "profile-default-image.jpg");
                    byte[] imagemBytes = System.IO.File.ReadAllBytes(completePath);

                    if (user == null)
                    {
                        user = new User
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Username,
                            UserName = model.Username,
                            Address = model.Address,
                            PostalCode = model.PostalCode,
                            Location = model.Location,
                            PictureSource = imagemBytes
                        };

                        var result = await _userHelper.AddUserAsync(user, "Aa1234567890");

                        if (result != IdentityResult.Success)
                        {
                            ModelState.AddModelError(string.Empty, "The user couldn´t be created.");
                            return View(model);
                        }

                        var token = await _userHelper.GenerateConfirmEmailTokenAsync(user);

                        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = model.Username }, Request.Scheme);

                        _emailHelper.SendEmail($"{model.Username}", $"Welcome to WordHeaven", $"Mr. /Ms. {user.FirstName} {user.LastName},<br/><br/> " +
                             $"Welcome to WordHeaven!<br/><br/> Here's your login and password: <br/><br/> Login: {model.Username}<br/>Password: Aa1234567890" +
                             "<br/><br/>For your security, it is recommended that you change your password.<br/>To do this, go to: <b>My Account >> Change password</b>" +
                             $"<br/><br/> " +
                             $"To confirm your account, click on this link: " +
                             $"<a href = \"{confirmationLink}\">Account Confirmation</a>" +
                             "<br/><br/>Best regards, " +
                             "<br/>WordHeaven");


                        ModelState.AddModelError(string.Empty, "The user as been created.");
                        return this.View();
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
                return View("Error");

            if (token == String.Empty)
                return View("Error");

            await _userHelper.EmailConfirmAsync(user, token);
            await _userHelper.LogoutAsync();
            return View();
        }


        #endregion Register

        #region Change Personal Data

        [Authorize]
        public async Task<IActionResult> ChangePersonalInformation()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = new ChangePersonalInformationViewModel();

            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Email = user.Email;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePersonalInformation(ChangePersonalInformationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;

                    if (model.PictureUser != null && model.PictureUser.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.PictureUser.CopyToAsync(memoryStream);

                            // If to controll the file size less (2 MB)
                            if (memoryStream.Length < 2097152)
                            {
                                user.PictureSource = memoryStream.ToArray();
                            }
                            else
                            {
                                ModelState.AddModelError("File", "The file is too large.");
                            }
                        }
                    }

                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                        this.ViewBag.Message = "Your Personal Data has been updated!";
                    else
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                }
            }
            return View(model);
        }

        #endregion Change Personal Data

        #region Change/Recover Password

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                    if (result.Succeeded)
                    {
                        await _userHelper.LogoutAsync();
                        return RedirectToAction("Login", "Account");
                    }
                    else
                        ViewBag.Message = "Invalid password! Please, enter your current password correctly.";
                }
            }
            return View(model);
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The e-mail doesn't correspont to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = this.Url.Action("ResetPassword", "Account", new { token = myToken }, protocol: HttpContext.Request.Scheme);

                Responses response = _emailHelper.SendEmail(user.Email, "WordHeaven Reset Password<br/>",
                      $"Mr. /Mrs. {user.FirstName} {user.LastName},<br/> " +
                      $"To reset your password click on the link below:<br/><br/>" +
                      $"<a href = \"{link}\">Reset Password</a>" +
                       "<br/><br/>Best regards, " +
                       "<br/>WordHeaven");

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to recover your password has been sent to the email associated.";
                }

                return this.View();
            }

            return this.View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.NewPassword);

                if (result.Succeeded)
                {
                    this.ViewBag.Message = "Password reset successful.";
                    return this.View();
                }

                this.ViewBag.Message = "Error while resetting the password.";
                return View(model);
            }

            this.ViewBag.Message = "User not found!";
            return View(model);
        }

        #endregion Change/Recover Password

        #region Help

        [Authorize]
        public IActionResult Help()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Help(HelpViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                    _emailHelper.SendEmailWithCC("WordsHeaven@outlook.pt",user.Email, $"Report with Severity {model.Severity}: {model.Subject}",
                         $"Mr. /Ms. {user.FullName} reports in {DateTime.Now}," +
                         $"<br/><br/>{model.ProblemDescription}" +
                         "</br><br/><br/>Best regards, " +
                         $"<br/>{user.FullName}");


                    ModelState.AddModelError(string.Empty, "The ticket as been created.");
                    return this.View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View(model);
            }
            return View(model);
        }

        #endregion

        public IActionResult NotAuthorized()
        {
            return View();
        }

    }
}








