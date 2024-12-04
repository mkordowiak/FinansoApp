using FinansoApp.Controllers;
using FinansoApp.ViewModels;
using FinansoData;
using FinansoData.Models;
using FinansoData.Repository.Account;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;

namespace FinansoApp.Tests.Controllers
{
    public class AccountControllerTest
    {
        private readonly Mock<IUserStore<AppUser>> _userStoreMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
        private readonly Mock<IUserClaimsPrincipalFactory<AppUser>> _userPrincipalFactoryMock;
        private readonly Mock<SignInManager<AppUser>> _signInManagerMock;
        private readonly Mock<IAuthentication> _authenticationMock;
        private readonly Mock<IUserManagement> _userManagementMock;
        private readonly Mock<IUserQuery> _userQueryMock;


        public AccountControllerTest()
        {
            _userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser { UserName = "testuser" });

            _userQueryMock = new Mock<IUserQuery>();

            _contextAccessorMock = new Mock<IHttpContextAccessor>();
            _userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<AppUser>>();

            _authenticationMock = new Mock<IAuthentication>();
            _userManagementMock = new Mock<IUserManagement>();



            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                _contextAccessorMock.Object,
                _userPrincipalFactoryMock.Object,
                null,
                null,
                null,
                null);


            _signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                     .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        }



        [Fact]
        public async Task AccountController_Login_ShouldLoginOnlyWhenCredentialsAreCorrect()
        {
            #region Arrange

            // Input data
            string email = "test@mail.com";
            string correctPassword = "correctPassword";


            // Input ViewModels
            LoginViewModel correctViewModel = new LoginViewModel
            {
                Email = email,
                Password = correctPassword,
            };


            RepositoryResult<AppUser?> loginAsyncUser = RepositoryResult<AppUser?>
                .Success(new AppUser());

            // Mock 
            _authenticationMock.Setup(x => x.LoginAsync(email, correctPassword))
                .ReturnsAsync(loginAsyncUser);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object);


            RepositoryResult<AppUser?> loginAsyncMock = RepositoryResult<AppUser?>.Success(new AppUser());

            _authenticationMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(loginAsyncMock);

            #endregion


            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult correctCredentialsResult = await accountController.Login(correctViewModel);
            #endregion


            #region ASSERT
            correctCredentialsResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>();
            #endregion

        }


        [Fact]
        public async Task AccountController_Login_ShouldReturnErrorWhenCredentialsAreWrong()
        {
            #region Arrange

            // Input data
            string email = "test@mail.com";
            string incorrectPassword = "incorrectPassword";


            // Input ViewModels
            LoginViewModel incorrectViewModel = new LoginViewModel
            {
                Email = email,
                Password = incorrectPassword,
            };




            // Mock 
            RepositoryResult<AppUser?> loginAsyncMock = RepositoryResult<AppUser?>.Failure(null, ErrorType.WrongPassword);
            _authenticationMock.Setup(x => x.LoginAsync(email, incorrectPassword))
                .ReturnsAsync(loginAsyncMock);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object);


            #endregion


            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult incorrectCredentialsRestult = await accountController.Login(incorrectViewModel);
            #endregion


            #region ASSERT
            incorrectCredentialsRestult.Should().BeOfType<Microsoft.AspNetCore.Mvc.ViewResult>();

            LoginViewModel? incorrectCredentialsReturnedViewModel = (incorrectCredentialsRestult as Microsoft.AspNetCore.Mvc.ViewResult).Model as LoginViewModel;

            incorrectCredentialsReturnedViewModel.Error.WrongCredentials.Should().BeTrue();
            #endregion

        }




        [Fact]
        public async Task AccountController_Register_ShouldReturnErrorWhenEmailAlreadyExists()
        {
            #region Arrange

            // Input data
            string email = "test@mail.com";
            string password = "Password";
            string confirmPassword = password;



            // Input ViewModels
            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };




            // Mock 
            RepositoryResult<AppUser?> createAppUserResult = RepositoryResult<AppUser?>.Failure(null, ErrorType.EmailAlreadyExists);
            _userManagementMock.Setup(x => x.CreateAppUser(email, password))
                .ReturnsAsync(createAppUserResult);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object);
            #endregion


            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult registerActionResult = await accountController.Register(registerViewModel);
            #endregion


            #region ASSERT
            registerActionResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.ViewResult>();
            RegisterViewModel? registerReturnedViewModel = (registerActionResult as Microsoft.AspNetCore.Mvc.ViewResult).Model as RegisterViewModel;
            registerReturnedViewModel.Error.AlreadyExists.Should().BeTrue();
            #endregion
        }


        [Fact]
        public async Task AccountController_Register_ShouldReturnErrorWhenWhenAssignUserRoleError()
        {
            #region Arrange

            // Input data
            string email = "test@mail.com";
            string password = "Password";
            string confirmPassword = password;



            // Input ViewModels
            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };


            RepositoryResult<AppUser?> createUserMock = RepositoryResult<AppUser?>.Failure(null, ErrorType.AssignUserRoleError);


            // Mock 
            _userManagementMock.Setup(x => x.CreateAppUser(email, password))
                .ReturnsAsync(createUserMock);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object);

            #endregion


            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult registerActionResult = await accountController.Register(registerViewModel);
            #endregion


            #region ASSERT
            registerActionResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.ViewResult>();
            RegisterViewModel? registerReturnedViewModel = (registerActionResult as Microsoft.AspNetCore.Mvc.ViewResult).Model as RegisterViewModel;
            registerReturnedViewModel.Error.CreateUserError.Should().BeTrue();
            #endregion
        }




        [Fact]
        public async Task AccountController_Register_ShouldReturnRedirectToActionWhenOK()
        {
            #region Arrange

            // Input data
            string email = "test@mail.com";
            string password = "Password";
            string confirmPassword = password;



            // Input ViewModels
            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };
            AppUser user = new AppUser { };

            RepositoryResult<AppUser?> createAppUserResult = RepositoryResult<AppUser?>.Success(new AppUser { });


            // Mock 
            _userManagementMock.Setup(x => x.CreateAppUser(email, password))
                .ReturnsAsync(createAppUserResult);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object);

            #endregion


            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult registerActionResult = await accountController.Register(registerViewModel);
            #endregion


            #region ASSERT
            registerActionResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>();
            #endregion
        }

        #region Edit_HttpGet

        [Fact]
        public async Task AccountController_Edit_HttpGet_ShouldBeAuthorized()
        {
            #region Arrange
            MethodInfo httpGetMethodInfo = typeof(AccountController).GetMethod(nameof(AccountController.Edit), Type.EmptyTypes);
            #endregion

            #region Act
            AuthorizeAttribute? httpGetAuthorizeAttribute = httpGetMethodInfo.GetCustomAttribute<AuthorizeAttribute>();

            #endregion

            #region Assert
            httpGetAuthorizeAttribute.Should().NotBeNull("this method should be protected by authorization");
            #endregion
        }

        [Fact]
        public async Task AccountController_Edit_HttpGet_ShouldReturnNotFoundWhenCantFindUser()
        {
            #region Arrange
            // Input data
            string appUserId = "appuserid";
            string email = "test@test.pl";
            AppUser appUser = new AppUser { Id = appUserId, Email = email, NormalizedEmail = email };


            // Repository Mock
            _userQueryMock.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<AppUser?>.Success(null));

            // Claims Principal Mock
            Mock<System.Security.Claims.ClaimsPrincipal> mockPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(email);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            #endregion

            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult result = await accountController.Edit();
            #endregion

            #region ASSERT
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.NotFoundResult>();
            #endregion
        }

        [Fact]
        public async Task AccountController_Edit_HttpGet_ShouldReturnView()
        {
            #region Arrange
            // Input data
            string appUserId = "appuserid";
            string email = "test@test.pl";
            AppUser appUser = new AppUser { Id = appUserId, Email = email, NormalizedEmail = email };


            // Repository Mock
            _userQueryMock.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<AppUser?>.Success(appUser));

            // Claims Principal Mock
            Mock<System.Security.Claims.ClaimsPrincipal> mockPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(email);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            #endregion

            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult result = await accountController.Edit();
            #endregion

            #region ASSERT
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.ViewResult>();
            #endregion
        }

        #endregion

        #region Edit_HttpPost

        [Fact]
        public async Task AccountController_Edit_HttpPost_ShouldBeAuthorized()
        {
            #region Arrange
            MethodInfo httpGetMethodInfo = typeof(AccountController).GetMethod(nameof(AccountController.Edit), new[] {typeof(FinansoApp.ViewModels.Account.EditAccountViewModel) } );
            #endregion

            #region Act
            AuthorizeAttribute? httpGetAuthorizeAttribute = httpGetMethodInfo.GetCustomAttribute<AuthorizeAttribute>();

            #endregion

            #region Assert
            httpGetAuthorizeAttribute.Should().NotBeNull("this method should be protected by authorization");
            #endregion
        }


        [Fact]
        public async Task AccountController_Edit_HttpPost_ShouldReturnViewIfModelIsInvalid()
        {
            #region Arrange
            FinansoApp.ViewModels.Account.EditAccountViewModel editAccountViewModel = new FinansoApp.ViewModels.Account.EditAccountViewModel
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Nickname = "Nickname"
            };

            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object);

            accountController.ModelState.AddModelError("FirstName", "FirstName is required");

            #endregion

            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult result = await accountController.Edit(editAccountViewModel);
            #endregion

            #region ASSERT
            result.Should().BeOfType<ViewResult>();

            #endregion
        }


        [Fact]
        public async Task AccountController_Edit_HttpPost_ShouldReturnNotFoundIfCantFindUser()
        {
            #region Arrange
            // Input data
            string appUserId = "appuserid";
            string email = "test@test.pl";
            AppUser appUser = new AppUser { Id = appUserId, Email = email, NormalizedEmail = email };

            FinansoApp.ViewModels.Account.EditAccountViewModel editAccountViewModel = new FinansoApp.ViewModels.Account.EditAccountViewModel
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Nickname = "Nickname"
            };

            // Repository Mock
            _userQueryMock.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<AppUser?>.Success(null));

            // Claims Principal Mock
            Mock<System.Security.Claims.ClaimsPrincipal> mockPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(email);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            #endregion

            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult result = await accountController.Edit(editAccountViewModel);
            #endregion

            #region ASSERT
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.NotFoundResult>();
            #endregion
        }

        [Fact]
        public async Task AccountController_Edit_HttpPost_ShouldRedirectToActionWhenOk()
        {
            #region Arrange
            // Input data
            string appUserId = "appuserid";
            string email = "test@test.pl";
            AppUser appUser = new AppUser { Id = appUserId, Email = email, NormalizedEmail = email };

            FinansoApp.ViewModels.Account.EditAccountViewModel editAccountViewModel = new FinansoApp.ViewModels.Account.EditAccountViewModel
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Nickname = "Nickname"
            };

            // Repository Mock
            _userQueryMock.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<AppUser?>.Success(appUser));
            _userManagementMock.Setup(x => x.EditUserInfo(It.IsAny<AppUser>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));

            // Claims Principal Mock
            Mock<System.Security.Claims.ClaimsPrincipal> mockPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(email);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _authenticationMock.Object,
                _userManagementMock.Object,
                _userQueryMock.Object,
                _signInManagerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            #endregion

            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult result = await accountController.Edit(editAccountViewModel);
            #endregion

            #region ASSERT
            result.Should().BeOfType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>();

            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;
            redirectToActionResult.ActionName.Should().Be("Index");
            redirectToActionResult.ControllerName.Should().Be("Home");

            #endregion
        }

        #endregion
    }
}
