using Microsoft.AspNetCore.Mvc;
using FinansoApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using FinansoData.Models;
using Microsoft.EntityFrameworkCore;
using FinansoData.Data;
using FinansoData.BLL;

namespace FinansoApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAccountBLL _accountBLL;

        public AccountController(UserManager<AppUser> userManager, IAccountBLL accountBLL, SignInManager<AppUser> signInManager = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountBLL = accountBLL;
        }

        


        public IActionResult Login()
        {
            // Hold values wher reload
            var responseViewModel = new LoginViewModel();
            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(!ModelState.IsValid) return View(loginViewModel);


            // Checks if user exists
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (user == null)
            {
                TempData["Error"] = false;
                return View(loginViewModel);
            }
            

            // Check if password matches
            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
            if(passwordCheck == false)
            {
                TempData["Error"] = false;
                return View(loginViewModel);
            }

            // Perform login
            var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
            if (result.Succeeded == false)
            {
                TempData["Error"] = false;
                return View(loginViewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            // Hold values wher reload
            var responseViewModel = new RegisterViewModel();
            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);


            AppUser user;
            try
            {
                user = await _accountBLL.RegisterNewUserAsync(registerViewModel.Email, registerViewModel.Password);
            }
            catch
            {
                TempData["RegisterError"] = true;
                return View(registerViewModel);
            }
            

            if(_accountBLL.RegisterAlreadyExists == true)
            {
                TempData["AlreadyExists"] = true;
                return View(registerViewModel);
            }

            if (_accountBLL.RegisterError == true)
            {
                TempData["RegisterError"] = true;
                return View(registerViewModel);
            }


            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
