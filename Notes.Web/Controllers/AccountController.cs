using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AccountController(UserManager<UserEntry> userManager, SignInManager<UserEntry> signInManager, IAccountInfoManager accountInfoManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._accountInfoManager = accountInfoManager;
            this._mapper = mapper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserEntry user = new UserEntry { Email = model.Email, UserName = model.Email, Nickname = model.Nickname };

                if (model.Email == null || model.Password == null)
                    throw new ArgumentException("Email and Password can't be null");

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
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
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Email == null || model.Password == null)
                    throw new ArgumentException("Email and Password can't be null");

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
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
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [Authorize]
        public IActionResult Info()
        {
            var accountInfo = _accountInfoManager.GetAccountInfo();

            var viewModel = _mapper.Map<AccountInfo, AccountInfoViewModel>(accountInfo);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
