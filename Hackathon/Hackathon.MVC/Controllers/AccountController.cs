using Hackathon.Application.Companies;
using Hackathon.Domain.UserAggregate;
using Hackathon.MVC.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Hackathon.MVC.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ICompanyService _companyService;
        private readonly IEmailSender _emailSender;

        public AccountController(
            ILogger<AccountController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ICompanyService companyService,
            IEmailSender emailSender)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _companyService = companyService;
            _emailSender = emailSender;
        }


        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var vm = new LoginViewModel();
            if (!string.IsNullOrEmpty(vm.ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, vm.ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            vm.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            vm.ReturnUrl = returnUrl;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe,
                        lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Neveljaven poskus prijave.");
                return View(vm);
            }

            // If we got this far, something failed, redisplay form
            return View(vm);
        }

        ///// <summary>
        ///// Entry point into the register workflow
        ///// </summary>
        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl)
        {
            var vm = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var companies = (await _companyService.GetAllAsync());
                var newCompany = new CompanyDto();
                if (companies.All(x => x.Name != vm.Company))
                {
                    newCompany = new CompanyDto()
                    {
                        Id = Guid.NewGuid(),
                        Name = vm.Company
                    };
                }

                var company = companies.Any(x => x.Name == vm.Company)
                    ? companies.FirstOrDefault(x => x.Name == vm.Company)
                    : await _companyService.CreateAsync(newCompany);

                var user = new User()
                {
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    UserName = vm.Email,
                    Email = vm.Email,
                    CompanyId = company?.Id
                };
                var result = await _userManager.CreateAsync(user, vm.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new {area = "", userId = user.Id, code = code},
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(vm.Email, "Potrdite vaš elektronski naslov",
                        $"Prosimo potrdite vaš elektronski naslov s <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>klikom na povezavo</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var vm = new ConfirmEmailModel();
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            vm.StatusMessage = result.Succeeded
                ? "Zahvaljujemo se vam za potrditev e-pošte."
                : "Napaka pri potrditvi e-pošte.";
            return View(vm);
        }

        /// <summary>
        /// Entry point into the forgot password workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ForgotPassword(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return Redirect(nameof(ForgotPasswordConfirmation));
                }

                await SendPasswordResetEmail(vm, await _userManager.GeneratePasswordResetTokenAsync(user));

                return Redirect(nameof(ForgotPasswordConfirmation));
            }

            return View(vm);
        }

        private async Task SendPasswordResetEmail(ForgotPasswordModel vm, string code)
        {
            var callbackUrl = Url.Action(
                "ResetPassword",
                "Account",
                values: new {area = "", code},
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                vm.Email,
                "Ponastavitev gesla",
                $"Prosimo, ponastavite geslo z <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>klikom tukaj</a>.");
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPasswordConfirmation(string returnUrl)
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("Za ponastavitev gesla morate navesti kodo.");
            }
            else
            {
                var vm = new ResetPasswordModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                return Redirect(nameof(ForgotPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, vm.Code, vm.Password);
            if (result.Succeeded)
            {
                return Redirect(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);
        }


        /// <summary>
        /// Entry point into the logout workflow
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            return Redirect(nameof(Login));
        }


        //// GET: Accounts
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _userService.GetAllAsync());
        //}

        //// GET: Accounts/Details/5
        //public async Task<IActionResult> Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _userService.GetAsync((Guid) id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}

        //// GET: Accounts/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Accounts/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(
        //    [Bind(
        //        "IsDeleted,DeletionTime,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")]
        //    UserDto user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        user.Id = Guid.NewGuid();
        //        await _userService.CreateAsync(user);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(user);
        //}

        //// GET: Accounts/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _userService.GetAsync((Guid) id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}

        //// POST: Accounts/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id,
        //    [Bind(
        //        "IsDeleted,DeletionTime,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")]
        //    UserDto user)
        //{
        //    if (id != user.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _userService.UpdateAsync(user);
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!await UserExists(user.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(user);
        //}

        //// GET: Accounts/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _userService.GetAsync((Guid) id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}

        //// POST: Accounts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var user = await _userService.GetAsync(id);
        //    await _userService.RemoveAsync(user.Id);
        //    return RedirectToAction(nameof(Index));
        //}
    }
}