using Mailing;
using Mailing.MailKitImplementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pb304PetShop.DataContext.Entities;
using Pb304PetShop.Models;

namespace Pb304PetShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;


        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMailService mailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _configuration = configuration;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new AppUser
            {
                UserName = registerViewModel.Username,
                Email = registerViewModel.Email,
                FullName = registerViewModel.FullName,
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string link = Url.Action("ConfirmEmail", "Account", new { email = user.Email, token },
            HttpContext.Request.Scheme, HttpContext.Request.Host.Value);
            string body = $"<a href='{link}'>Confirm Email</a>";

            MailKitMailService mailKitMailService = new MailKitMailService(_configuration);

            mailKitMailService.SendMail(new Mail
             {
                ToEmail = user.Email,
                Subject = "Confirm Email",
                HtmlBody = body,
                ToFullName = user.FullName,
                TextBody = body,
                Attachments = null,


            });

            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var existUser = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if (existUser == null) 
            {
                ModelState.AddModelError("", "Username or password is incorrect");

                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(existUser, loginViewModel.Password, loginViewModel.RememberMe, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", $"You are banned {existUser.LockoutEnd.Value - DateTimeOffset.UtcNow}");
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password is incorrect");

                return View();
            }

            if (loginViewModel.ReturnUrl != null)
            {
                return Redirect(loginViewModel.ReturnUrl);
            }
            if (!await _userManager.IsEmailConfirmedAsync(existUser))
            {
                ModelState.AddModelError("", "Please confirm email");
                return View();
            }

            var signInManager = await _signInManager.PasswordSignInAsync(existUser, loginViewModel.Password, loginViewModel.RememberMe, true);
            if (signInManager.IsLockedOut)
            {
                ModelState.AddModelError("", "Blok edildi...");
                return View();
            }
            if (!signInManager.Succeeded)
            {
                ModelState.AddModelError("", "username/email or password is incorrect");
                return View();

            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.CurrentPassword, changePasswordViewModel.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            return RedirectToAction(nameof(Login));
        }
   
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ModelState.AddModelError("", "Email doesnt found");
                return View();
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword", "Account", new { resetToken, email }, Request.Scheme, Request.Host.ToString());

            _mailService.SendMail(new Mail { ToEmail = email, Subject = "Reset pas",TextBody = resetLink});

            return View(nameof(EmailSimulyasiya), resetLink);
        }

        public IActionResult EmailSimulyasiya()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordViewModel);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);

            if (user == null)
                return BadRequest();

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.ResetToken, resetPasswordViewModel.NewPassword);

            if(!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

                return View(resetPasswordViewModel);
            }

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel confirmEmailViewModel)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailViewModel.Email);
            if (user == null)
                return NotFound();

            if (await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest();

            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user, confirmEmailViewModel.Token);
            if (identityResult.Succeeded)
            {
                TempData["ConfirmationMessage"] = "Your email successfully confirmed";
                return RedirectToAction(nameof(Login));
            }



            return BadRequest();
        }

    }
}
