using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using Moq;
using FinansoApp.Controllers;
using Microsoft.AspNetCore.Http;
using FinansoApp.ViewModels;
using System.Runtime.CompilerServices;
using FluentAssertions;

namespace FinansoApp.Tests.Controllers
{
    public class AccountControllerTest
    {




        [Fact]
        public async Task AccountController_Login_ShoudLoginWhenCredentialsAreCorrect()
        {
            string email = "test@mail.com";
            var correctPassword = "correctPassword";
            var incorrectPassword = "incorrectPassword";

            var userStoreMock = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<AppUser>>();

            var signInManagerMock = new Mock<SignInManager<AppUser>>(
                userManagerMock.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                null,
                null,
                null,
                null);

            AppUser user = new AppUser();

            signInManagerMock.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                     .ReturnsAsync(SignInResult.Success);

            signInManagerMock.Setup( x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);




            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser { UserName = "testuser" });

            

            var accountRepositoryMock = new Mock<IAccountRepository>();

            accountRepositoryMock.Setup(x => x.LoginAsync(email, correctPassword))
                .ReturnsAsync(user);


            AccountController accountController = new AccountController(
                userManagerMock.Object,
                accountRepositoryMock.Object,
                signInManagerMock.Object);






            // Login VM
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
            //ACT
            var correctResult = await accountController.Login(correctViewModel);
            var incorrectRestult = await accountController.Login(incorrectViewModel);
            

            correctResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>();
            incorrectRestult.Should().BeOfType < Microsoft.AspNetCore.Mvc.ViewResult> ();

            var aaa = incorrectRestult as Microsoft.AspNetCore.Mvc.ViewResult;
            var bbb = aaa.Model as LoginViewModel;

            bbb.ErrorMessages.WrongCredentials.Should().BeTrue();




            correctResult.Should().NotBeNull();
            
        }


    }
}
