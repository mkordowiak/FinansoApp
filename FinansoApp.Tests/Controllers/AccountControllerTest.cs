﻿using FinansoApp.Controllers;
using FinansoApp.ViewModels;
using FinansoData.Models;
using FinansoData.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FinansoApp.Tests.Controllers
{
    public class AccountControllerTest
    {
        private readonly Mock<IUserStore<AppUser>> _userStoreMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
        private readonly Mock<IUserClaimsPrincipalFactory<AppUser>> _userPrincipalFactoryMock;
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<SignInManager<AppUser>> _signInManagerMock;

        public AccountControllerTest()
        {
            _userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser { UserName = "testuser" });

            _contextAccessorMock = new Mock<IHttpContextAccessor>();
            _userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<AppUser>>();

            _accountRepositoryMock = new Mock<IAccountRepository>();

            try
            {
                _accountRepositoryMock.Setup(x => x.Err.IsError())
                .Returns(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            




            _signInManagerMock = new Mock<SignInManager<AppUser>>(
                _userManagerMock.Object,
                _contextAccessorMock.Object,
                _userPrincipalFactoryMock.Object,
                null,
                null,
                null,
                null);

            _signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                     .ReturnsAsync(SignInResult.Success);

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);




        }



        [Fact]
        public async Task AccountController_Login_ShouldLoginOnlyWhenCredentialsAreCorrect()
        {
            #region Arrange

            // Input data
            string email = "test@mail.com";
            string correctPassword = "correctPassword";
            string incorrectPassword = "incorrectPassword";


            // Input ViewModels
            LoginViewModel correctViewModel = new LoginViewModel
            {
                Email = email,
                Password = correctPassword,
            };
            LoginViewModel incorrectViewModel = new LoginViewModel
            {
                Email = email,
                Password = incorrectPassword,
            };




            // Mock 
            _accountRepositoryMock.Setup(x => x.LoginAsync(email, correctPassword))
                .ReturnsAsync(new AppUser());

            _accountRepositoryMock.Setup(x => x.LoginAsync(email, incorrectPassword))
                .ReturnsAsync((AppUser)null);


            // Arrange controller
            AccountController accountController = new AccountController(
                _userManagerMock.Object,
                _accountRepositoryMock.Object,
                _signInManagerMock.Object);

            #endregion


            #region ACT
            Microsoft.AspNetCore.Mvc.IActionResult correctCredentialsResult = await accountController.Login(correctViewModel);
            Microsoft.AspNetCore.Mvc.IActionResult incorrectCredentialsRestult = await accountController.Login(incorrectViewModel);
            #endregion


            #region ASSERT
            correctCredentialsResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>();
            incorrectCredentialsRestult.Should().BeOfType<Microsoft.AspNetCore.Mvc.ViewResult>();

            LoginViewModel? incorrectCredentialsReturnedViewModel = (incorrectCredentialsRestult as Microsoft.AspNetCore.Mvc.ViewResult).Model as LoginViewModel;
           
            incorrectCredentialsReturnedViewModel.Error.WrongCredentials.Should().BeTrue();
            #endregion

        }


    }
}
