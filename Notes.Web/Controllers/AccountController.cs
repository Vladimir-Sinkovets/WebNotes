using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notes.BLL.Services.AccountInfoManagers;
using Notes.BLL.Services.AccountInfoManagers.Models;
using Notes.DAL.Models;
using Notes.Web.Models.Account;
using System;
using System.Threading.Tasks;

namespace Notes.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<UserEntry> _userManager;
        private readonly SignInManager<UserEntry> _signInManager;
        private readonly IAccountInfoManager _accountInfoManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<UserEntry> userManager, SignInManager<UserEntry> signInManager, IAccountInfoManager accountInfoManager,
            IMapper mapper, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountInfoManager = accountInfoManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogDebug($"\"Register\" action method called");
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogDebug($"\"Register\" action method called");

            if (ModelState.IsValid)
            {
                var user = new UserEntry { Email = model.Email, UserName = model.Email, Nickname = model.Nickname };

                if (model.Email == null || model.Password == null)
                    throw new ArgumentException("Email and Password can't be null");

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogDebug($"Register successfully: {model.Email}");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogDebug($"Error loading user: {model.Email}");

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            _logger.LogDebug($"\"Login\" action method called");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogDebug($"\"Login\" action method called");

            if (ModelState.IsValid)
            {
                if (model.Email == null || model.Password == null)
                    throw new ArgumentException("Email and Password can't be null");

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogDebug($"Login successfuly {result}");

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    _logger.LogDebug($"Login failed: {model.Email}");

                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [Authorize]
        public IActionResult Info()
        {
            _logger.LogDebug($"\"Info\" action method called");

            var accountInfo = _accountInfoManager.GetAccountInfo();

            var viewModel = _mapper.Map<AccountInfo, AccountInfoViewModel>(accountInfo);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogDebug($"\"Logout\" action method called");

            await _signInManager.SignOutAsync();
            
            return RedirectToAction("Index", "Home");
        }
    }
}
