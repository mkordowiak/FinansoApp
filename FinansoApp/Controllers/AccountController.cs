﻿using FinansoApp.ViewModels;
using FinansoData.Models;
using FinansoData.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace FinansoApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAccountRepository _accountRepository;

        public AccountController(UserManager<AppUser> userManager, IAccountRepository accountRepository, SignInManager<AppUser> signInManager = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountRepository = accountRepository;
        }




        public IActionResult Login()
        {
            // Hold values wher reload
            var responseViewModel = new LoginViewModel();
            return View(responseViewModel);
        }

        public async Task<bool> test(string email)
        {
            bool aaa = await _accountRepository.IsUserExistsAsync(email);
            return aaa;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            AppUser? user = await _accountRepository.LoginAsync(loginViewModel.Email, loginViewModel.Password);

            // if there's something wrong with accessing data
            if (user == null
                && _accountRepository.Error.Any(x => x.Key == "DatabaseError"))
            {
                loginViewModel.ErrorMessages.InternalError = true;
                return View(loginViewModel);
            }

            // When app can access data, but credentials did not match
            if (user == null)
            {
                loginViewModel.ErrorMessages.WrongCredentials = true; ;
                return View(loginViewModel);
            }



            // Perform login
            var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
            if (result.Succeeded == false)
            {
                loginViewModel.ErrorMessages.InternalError = true;
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
                user = await _accountRepository.CreateAppUser(registerViewModel.Email, registerViewModel.Password);
            }
            catch
            {
                TempData["RegisterError"] = true;
                return View(registerViewModel);
            }



            // If email already exists pass information to frontend
            if (_accountRepository.Error.Any(x => x.Key == "EmailAlreadyExists"))
            {
                TempData["AlreadyExists"] = true;
                return View(registerViewModel);
            }

            if (_accountRepository.Error.Any(x => x.Key == "RegisterError"))
            {
                TempData["RegisterError"] = true;
                return View(registerViewModel);
            }

            if (_accountRepository.Error.Any(x => x.Key == "AssignUserRoleError"))
            {
                TempData["RegisterError"] = true;
                return View(registerViewModel);
            }

            if (user == null)
            {
                TempData["RegisterError"] = true;
                return View(registerViewModel);
            }


            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
