using FinansoApp.ViewModels;
using FinansoData;
using FinansoData.Models;
using FinansoData.Repository.Account;
using FinansoData.Repository.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserQuery _userQuery;
        private readonly IAuthentication _authentication;
        private readonly IUserManagement _userManagement;
        private readonly IGroupManagementRepository _groupManagementRepository;

        public AccountController(
            UserManager<AppUser> userManager,
            IAuthentication authentication,
            IUserManagement userManagement,
            IUserQuery userQuery,
            IGroupManagementRepository groupManagementRepository,
            SignInManager<AppUser> signInManager = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userQuery = userQuery;
            _authentication = authentication;
            _userManagement = userManagement;
            _groupManagementRepository = groupManagementRepository;
        }




        public IActionResult Login()
        {
            // Hold values when reload
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


            if (user.Value == null)
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
            // Hold values when reload
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


            RepositoryResult<AppUser?> user = await _userManagement.CreateAppUser(registerViewModel.Email, registerViewModel.Password, registerViewModel.Name);

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

            RepositoryResult<bool?> groupManagementRepositoryResult = await _groupManagementRepository.Add("Default group", registerViewModel.Email);

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Edit()
        {
            RepositoryResult<AppUser?> repositoryResult = await _userQuery.GetUserByEmail(User.Identity.Name);

            if (repositoryResult.IsSuccess == false)
            {
                return NotFound();
            }

            if (repositoryResult.Value == null)
            {
                return NotFound();
            }

            FinansoApp.ViewModels.Account.EditAccountViewModel editAccountViewModel = new FinansoApp.ViewModels.Account.EditAccountViewModel
            {
                FirstName = repositoryResult.Value.FirstName,
                LastName = repositoryResult.Value.LastName,
                Nickname = repositoryResult.Value.Nickname
            };

            return View(editAccountViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(FinansoApp.ViewModels.Account.EditAccountViewModel editAccountViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editAccountViewModel);
            }

            RepositoryResult<AppUser?> getUserRepositoryResult = await _userQuery.GetUserByEmail(User.Identity.Name);

            if (getUserRepositoryResult.IsSuccess == false)
            {
                return NotFound();
            }

            if (getUserRepositoryResult.Value == null)
            {
                return NotFound();
            }

            getUserRepositoryResult.Value.FirstName = editAccountViewModel.FirstName;
            getUserRepositoryResult.Value.LastName = editAccountViewModel.LastName;
            getUserRepositoryResult.Value.Nickname = editAccountViewModel.Nickname;

            RepositoryResult<bool> editUserRepositoryResult = await _userManagement.EditUserInfo(getUserRepositoryResult.Value);


            if (editUserRepositoryResult.IsSuccess == false && editUserRepositoryResult.ErrorType == ErrorType.ServerError)
            {
                editAccountViewModel.Error.InternalError = true;
                return View(editAccountViewModel);
            }


            return RedirectToAction("Index", "Home");
        }
    }
}
