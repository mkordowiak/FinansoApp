using FinansoApp.ViewModels;
using FinansoData;
using FinansoData.Models;
using FinansoData.Repository.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        //private readonly IAccountRepository _accountRepository;

        private readonly IAuthentication _authentication;
        private readonly IUserManagement _userManagement;

        public AccountController(UserManager<AppUser> userManager, IAuthentication authentication, IUserManagement userManagement, SignInManager<AppUser> signInManager = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_accountRepository = accountRepository;

            _authentication = authentication;
            _userManagement = userManagement;
        }




        public IActionResult Login()
        {
            // Hold values wher reload
            LoginViewModel responseViewModel = new LoginViewModel();
            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            RepositoryResult<AppUser?> user = await _authentication.LoginAsync(loginViewModel.Email, loginViewModel.Password);


            // if there's something wrong with accessing data
            if (user.IsSuccess == false && user.ErrorType == FinansoData.ErrorType.ServerError)
            {
                loginViewModel.Error.InternalError = true;
                return View(loginViewModel);
            }


            // When app can access data, but credentials did not match
            if (user.IsSuccess == false && user.ErrorType == FinansoData.ErrorType.WrongPassword)
            {
                loginViewModel.Error.WrongCredentials = true;
                return View(loginViewModel);
            }


            if (user == null)
            {
                loginViewModel.Error.InternalError = true;
                return View(loginViewModel);
            }

            // Perform login
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user.Value, loginViewModel.Password, false, false);
            if (result.Succeeded == false)
            {
                loginViewModel.Error.InternalError = true;
                return View(loginViewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            // Hold values wher reload
            RegisterViewModel responseViewModel = new RegisterViewModel();
            return View(responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }


            RepositoryResult<AppUser?> user = await _userManagement.CreateAppUser(registerViewModel.Email, registerViewModel.Password);

            if (user.IsSuccess == false && user.ErrorType == FinansoData.ErrorType.ServerError)
            {
                registerViewModel.Error.CreateUserError = true;
                return View(registerViewModel);
            }


            // If email already exists pass information to frontend
            if (user.IsSuccess == false && user.ErrorType == FinansoData.ErrorType.EmailAlreadyExists)
            {
                registerViewModel.Error.AlreadyExists = true;
                return View(registerViewModel);
            }

            if (user.IsSuccess == false &&
                (
                    user.ErrorType == ErrorType.RegisterError
                    || user.ErrorType == ErrorType.AssignUserRoleError
                ))
            {
                registerViewModel.Error.CreateUserError = true;
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
