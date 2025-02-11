using FinansoApp.Controllers;
using FinansoApp.ViewModels.Balance;
using FinansoData.Repository.Balance;
using FinansoData.Repository.Currency;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using System.Security.Claims;

namespace FinansoApp.Tests.Controllers
{
    public class BalanceControllerTest
    {
        private readonly Mock<IBalanceManagementRepository> _balanceManagmentRepositoryMock;
        private readonly Mock<ICurrencyQueryRepository> _currencyQueryRepositoryMock;
        private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;
        private readonly Mock<IGroupUsersQueryRepository> _groupUsersQueryRepositoryMock;
        private readonly Mock<IBalanceQueryRepository> _balanceQueryRepositoryMock;
        private readonly Mock<IBalanceSumAmount> _balanceSumAmountMock;

        public BalanceControllerTest()
        {
            _balanceManagmentRepositoryMock = new Mock<IBalanceManagementRepository>();
            _currencyQueryRepositoryMock = new Mock<ICurrencyQueryRepository>();
            _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();
            _groupUsersQueryRepositoryMock = new Mock<IGroupUsersQueryRepository>();
            _balanceManagmentRepositoryMock = new Mock<IBalanceManagementRepository>();
            _balanceQueryRepositoryMock = new Mock<IBalanceQueryRepository>();
            _balanceSumAmountMock = new Mock<IBalanceSumAmount>();
        }

        #region Index

        [Fact]
        public async Task BalanceController_Index_ShouldReturnView()
        {
            // Arrange
            IEnumerable<FinansoData.DataViewModel.Balance.BalanceViewModel> balanceViewModels = new List<FinansoData.DataViewModel.Balance.BalanceViewModel>()
            {
                new FinansoData.DataViewModel.Balance.BalanceViewModel { Id = 1, Name = "Bank 1", Amount = 1, Currency = new FinansoData.Models.Currency { Id = 1, Name = "PLN" }, Group = new FinansoData.Models.Group { Id = 1, Name = "Test group 1" } },
                new FinansoData.DataViewModel.Balance.BalanceViewModel { Id = 2, Name = "Bank 2", Amount = 2, Currency = new FinansoData.Models.Currency { Id = 2, Name = "USD" }, Group = new FinansoData.Models.Group { Id = 2, Name = "Test group 2" } }
            };

            _balanceQueryRepositoryMock.Setup(x => x.GetListOfBalancesForUser(It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Balance.BalanceViewModel>>.Success(balanceViewModels));
            _balanceSumAmountMock.Setup(x => x.GetBalancesSumAmountForUser(It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<decimal?>.Success(3));

            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult result = await controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        #endregion

        #region AddBalance GET

        [Fact]
        public async Task BalanceController_AddBalance_GET_ShouldReturnView()
        {
            // Arrange

            IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel> currencyViewModels = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
            {
                new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
            };

            IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel> groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
            {
                new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
            };

            _currencyQueryRepositoryMock.Setup(x => x.GetAllCurrencies()).ReturnsAsync(FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel>>.Success(currencyViewModels));
            _groupQueryRepositoryMock.Setup(x => x.GetUserGroups(It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>?>.Success(groups));

            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult result = await controller.AddBalance();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public async Task BalanceController_AddBalance_GET_ShouldReturnBadRequestWhenNoGroup()
        {
            // Arrange

            IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel> currencyViewModels = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
            {
                new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
            };

            IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel> groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>();

            _currencyQueryRepositoryMock.Setup(x => x.GetAllCurrencies()).ReturnsAsync(FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel>>.Success(currencyViewModels));
            _groupQueryRepositoryMock.Setup(x => x.GetUserGroups(It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>?>.Success(groups));

            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult result = await controller.AddBalance();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestResult>(result);
        }

        #endregion

        #region AddBalance POST

        [Fact]
        public async Task BalanceController_AddBalance_POST_ShouldRedirectToIndex()
        {
            // Arrange
            AddBalanceViewModel addBalanceViewModel = new AddBalanceViewModel
            {
                Currencies = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
                {
                    new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
                },
                Groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
                {
                    new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
                }
            };

            IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel> currencyViewModels = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
            {
                new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
            };

            IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel> groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
            {
                new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
            };



            _groupUsersQueryRepositoryMock.Setup(x => x.GetUserMembershipInGroupAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel>.Success(new FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel { IsMember = true, IsOwner = false }));
            _currencyQueryRepositoryMock.Setup(x => x.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.Models.Currency>.Success(new FinansoData.Models.Currency { Id = 1, Name = "PLN" }));
            _groupQueryRepositoryMock.Setup(x => x.GetGroupById(It.IsAny<int>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.Models.Group?>.Success(new FinansoData.Models.Group { Id = 1, Name = "Name" }));
            _balanceManagmentRepositoryMock.Setup(x => x.AddBalance(It.IsAny<FinansoData.DataViewModel.Balance.BalanceViewModel>())).ReturnsAsync(FinansoData.RepositoryResult<bool>.Success(true));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult result = await controller.AddBalance(addBalanceViewModel);

            // Assert

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToActionResult>();
            RedirectToActionResult redirectActionResult = (RedirectToActionResult)result;
            redirectActionResult.ActionName.Should().Be("Index");
            redirectActionResult.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task BalanceController_AddBalance_POST_ShouldReturnBadRequestWhenCantAddToDatabase()
        {
            // Arrange
            AddBalanceViewModel addBalanceViewModel = new AddBalanceViewModel
            {
                Currencies = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
                {
                    new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
                },
                Groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
                {
                    new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
                }
            };

            IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel> currencyViewModels = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
            {
                new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
            };

            IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel> groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
            {
                new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
            };



            _groupUsersQueryRepositoryMock.Setup(x => x.GetUserMembershipInGroupAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel>.Success(new FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel { IsMember = true, IsOwner = false }));
            _currencyQueryRepositoryMock.Setup(x => x.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.Models.Currency>.Success(new FinansoData.Models.Currency { Id = 1, Name = "PLN" }));
            _groupQueryRepositoryMock.Setup(x => x.GetGroupById(It.IsAny<int>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.Models.Group?>.Success(new FinansoData.Models.Group { Id = 1, Name = "Name" }));
            _balanceManagmentRepositoryMock.Setup(x => x.AddBalance(It.IsAny<FinansoData.DataViewModel.Balance.BalanceViewModel>())).ReturnsAsync(FinansoData.RepositoryResult<bool>.Failure(null, FinansoData.ErrorType.ServerError));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult result = await controller.AddBalance(addBalanceViewModel);

            // Assert

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task BalanceController_AddBalance_POST_ShouldReturnUnauthorizedWhenUserIsNotMemberOfGroup()
        {
            // Arrange
            AddBalanceViewModel addBalanceViewModel = new AddBalanceViewModel
            {
                Currencies = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
                {
                    new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
                },
                Groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
                {
                    new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
                }
            };

            IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel> currencyViewModels = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
            {
                new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
            };

            IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel> groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
            {
                new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
            };



            _groupUsersQueryRepositoryMock.Setup(x => x.GetUserMembershipInGroupAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel>.Success(new FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel { IsMember = false, IsOwner = false }));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult result = await controller.AddBalance(addBalanceViewModel);

            // Assert

            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task BalanceController_AddBalance_POST_ShouldReturnBadRequestWhenGetUserMembershipInGroupAsyncServerError()
        {
            // Arrange
            AddBalanceViewModel addBalanceViewModel = new AddBalanceViewModel
            {
                Currencies = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
                {
                    new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
                },
                Groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
                {
                    new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
                }
            };

            IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel> currencyViewModels = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
            {
                new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
            };

            IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel> groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
            {
                new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
            };



            _groupUsersQueryRepositoryMock.Setup(x => x.GetUserMembershipInGroupAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel>.Failure(null, FinansoData.ErrorType.ServerError));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult result = await controller.AddBalance(addBalanceViewModel);

            // Assert

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task BalanceController_AddBalance_POST_ShouldReturnBadRequestWhenCantGetGroupInfo()
        {
            // Arrange
            AddBalanceViewModel addBalanceViewModel = new AddBalanceViewModel
            {
                Currencies = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
                {
                    new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
                },
                Groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
                {
                    new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
                }
            };

            IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel> currencyViewModels = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
            {
                new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
            };

            IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel> groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
            {
                new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
            };


            _currencyQueryRepositoryMock.Setup(x => x.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.Models.Currency>.Success(new FinansoData.Models.Currency { Id = 1, Name = "PLN" }));
            _groupUsersQueryRepositoryMock.Setup(x => x.GetUserMembershipInGroupAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel>.Success(new FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel { IsMember = true, IsOwner = false }));
            _groupQueryRepositoryMock.Setup(x => x.GetGroupById(It.IsAny<int>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.Models.Group?>.Failure(null, FinansoData.ErrorType.ServerError));

            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult result = await controller.AddBalance(addBalanceViewModel);

            // Assert

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task BalanceController_AddBalance_POST_ShouldReturnBadRequestWhenCantGetCurrencyModelById()
        {
            // Arrange
            AddBalanceViewModel addBalanceViewModel = new AddBalanceViewModel
            {
                Currencies = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
                {
                    new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
                },
                Groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
                {
                    new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
                }
            };

            IEnumerable<FinansoData.DataViewModel.Currency.CurrencyViewModel> currencyViewModels = new List<FinansoData.DataViewModel.Currency.CurrencyViewModel>()
            {
                new FinansoData.DataViewModel.Currency.CurrencyViewModel { Id = 1, Name = "PLN" },
            };

            IEnumerable<FinansoData.DataViewModel.Group.GetUserGroupsViewModel> groups = new List<FinansoData.DataViewModel.Group.GetUserGroupsViewModel>()
            {
                new FinansoData.DataViewModel.Group.GetUserGroupsViewModel { Id = 1, Name = "Group 1" },
            };


            _currencyQueryRepositoryMock.Setup(x => x.GetCurrencyModelById(It.IsAny<int>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.Models.Currency>.Failure(null, FinansoData.ErrorType.ServerError));
            _groupUsersQueryRepositoryMock.Setup(x => x.GetUserMembershipInGroupAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel>.Success(new FinansoData.DataViewModel.Group.GetUserMembershipInGroupViewModel { IsMember = true, IsOwner = false }));
            _groupQueryRepositoryMock.Setup(x => x.GetGroupById(It.IsAny<int>())).ReturnsAsync(FinansoData.RepositoryResult<FinansoData.Models.Group?>.Success(new FinansoData.Models.Group { Id = 1, Name = "Name" }));

            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult result = await controller.AddBalance(addBalanceViewModel);

            // Assert

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }
        #endregion

        #region SetBalanceAmount GET

        [Fact]
        public async Task BalanceController_SetBalanceAmount_GET_ShouldReturnView()
        {
            // Arrange
            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(FinansoData.RepositoryResult<bool?>.Success(true));

            _balanceQueryRepositoryMock.Setup(x => x.GetBalance(It.IsAny<int>()))
                .ReturnsAsync(FinansoData.RepositoryResult<FinansoData.DataViewModel.Balance.BalanceViewModel>
                .Success(
                    new FinansoData.DataViewModel.Balance.BalanceViewModel
                    {
                        Id = 1,
                        Name = "Bank 1",
                        Amount = 1,
                        Currency = new FinansoData.Models.Currency { Id = 1, Name = "PLN" },
                        Group = new FinansoData.Models.Group { Id = 1, Name = "Test group 1" }
                    }));



            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };


            // Act
            IActionResult result = await controller.SetBalanceAmount(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();

            ((ViewResult)result).Model.Should().BeOfType<SetBalanceAmountViewModel>();
        }

        [Fact]
        public async Task BalanceController_SetBalanceAmount_GET_ShouldReturnUnauthorizedWhenNoAccess()
        {
            // Arrange
            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(FinansoData.RepositoryResult<bool?>.Success(false));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };


            // Act
            IActionResult result = await controller.SetBalanceAmount(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedResult>();

        }

        [Fact]
        public async Task BalanceController_SetBalanceAmount_GET_ShouldBeAuthorized()
        {
            #region Arrange
            MethodInfo httpGetMethodInfo = typeof(BalanceController).GetMethod(nameof(BalanceController.SetBalanceAmount), new[] { typeof(int) });
            #endregion

            #region Act
            AuthorizeAttribute? httpGetAuthorizeAttribute = httpGetMethodInfo.GetCustomAttribute<AuthorizeAttribute>();

            #endregion

            #region Assert
            httpGetAuthorizeAttribute.Should().NotBeNull("this method should be protected by authorization");
            #endregion
        }

        #endregion


        #region SetBalanceAmount POST

        [Fact]
        public async Task BalanceController_SetBalanceAmount_POST_ShouldBeAuthorized()
        {
            #region Arrange
            MethodInfo httpGetMethodInfo = typeof(BalanceController).GetMethod(nameof(BalanceController.SetBalanceAmount), new[] { typeof(SetBalanceAmountViewModel) });
            #endregion

            #region Act
            AuthorizeAttribute? httpGetAuthorizeAttribute = httpGetMethodInfo.GetCustomAttribute<AuthorizeAttribute>();

            #endregion

            #region Assert
            httpGetAuthorizeAttribute.Should().NotBeNull("this method should be protected by authorization");
            #endregion
        }

        [Fact]
        public async Task BalanceController_SetBalanceAmount_POST_ShouldReturnRedirectToIndex()
        {
            // Arrange
            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(FinansoData.RepositoryResult<bool?>.Success(true));

            _balanceManagmentRepositoryMock.Setup(x => x.SetBalanceAmount(It.IsAny<int>(), It.IsAny<decimal>()))
                .ReturnsAsync(FinansoData.RepositoryResult<bool?>.Success(true));

            SetBalanceAmountViewModel setBalanceAmountViewModel = new SetBalanceAmountViewModel
            {
                BalanceId = 1,
                Amount = 100,
                BalanceName = "Bank 1",
                GroupName = "Test group 1"
            };

            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult repositoryResult = await controller.SetBalanceAmount(setBalanceAmountViewModel);

            // Assert
            repositoryResult.Should().NotBeNull();
            repositoryResult.Should().BeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)repositoryResult).ActionName.Should().Be("Index");
            ((RedirectToActionResult)repositoryResult).ControllerName.Should().Be("Balance");
        }

        [Fact]
        public async Task BalanceController_SetBalanceAmount_POST_ShouldReturnUnauthorizedWhenNoAccess()
        {
            // Arrange
            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(FinansoData.RepositoryResult<bool?>.Success(false));

            SetBalanceAmountViewModel setBalanceAmountViewModel = new SetBalanceAmountViewModel
            {
                BalanceId = 1,
                Amount = 100,
                BalanceName = "Bank 1",
                GroupName = "Test group 1"
            };

            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            BalanceController controller = new BalanceController(_balanceManagmentRepositoryMock.Object, _currencyQueryRepositoryMock.Object, _groupQueryRepositoryMock.Object, _groupUsersQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _balanceSumAmountMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult repositoryResult = await controller.SetBalanceAmount(setBalanceAmountViewModel);

            // Assert
            repositoryResult.Should().NotBeNull();
            repositoryResult.Should().BeOfType<UnauthorizedResult>();
        }
        #endregion
    }
}
