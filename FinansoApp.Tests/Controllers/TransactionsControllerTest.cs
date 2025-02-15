using AutoMapper;
using FinansoApp.Controllers;
using FinansoApp.ViewModels.Transaction;
using FinansoData;
using FinansoData.DataViewModel.Transaction;
using FinansoData.Repository.Balance;
using FinansoData.Repository.Settings;
using FinansoData.Repository.Transaction;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using System.Security.Claims;

namespace FinansoApp.Tests.Controllers
{
    public class TransactionsControllerTest
    {
        private Mock<ITransactionsQueryRepository> _transactionQueryRepositoryMock;
        private Mock<ITransactionManagementRepository> _transactionManagementRepositoryMock;
        private Mock<ITransactionMetaQueryRepository> _transactionMetaQueryRepositoryMock;
        private Mock<IBalanceQueryRepository> _balanceQueryRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISettingsQueryRepository> _settingsQueryRepositoryMock;

        public TransactionsControllerTest()
        {
            _transactionManagementRepositoryMock = new Mock<ITransactionManagementRepository>();
            _transactionQueryRepositoryMock = new Mock<ITransactionsQueryRepository>();
            _transactionMetaQueryRepositoryMock = new Mock<ITransactionMetaQueryRepository>();
            _balanceQueryRepositoryMock = new Mock<IBalanceQueryRepository>();
            _mapperMock = new Mock<IMapper>();
            _settingsQueryRepositoryMock = new Mock<ISettingsQueryRepository>();
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfTransactions()
        {
            // Arrange

            #region Mocking ClaimsPrincipal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            IEnumerable<GetTransactionsForUser> listOfTransactions = new List<GetTransactionsForUser>
            {
                new GetTransactionsForUser { TransactionId = 1, Amount = 100, GroupId = 1, GroupName = "Group Name", BalanceId = 1, BalanceName = "Balance name", TransactionType = "Income", TransactionStatus = "Planned", TransactionDate = DateTime.Now, CurrencyId = 1, CurrencyCode = "USD", CurrencyName = "Dolar", Description = String.Empty }
            };
            #endregion

            // Mocking repository method
            _transactionQueryRepositoryMock.Setup(
                x => x.GetTransactionsForUserUser(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(FinansoData.RepositoryResult<IEnumerable<GetTransactionsForUser>>
                    .Success(listOfTransactions, 1)
                );

            // Mocking settings
            _settingsQueryRepositoryMock.Setup(x => x.GetSettingsAsync<int>("TransactionListPageSize")).ReturnsAsync(10);

            // Mocking mapper
            _mapperMock.Setup(x => x.Map<FinansoApp.ViewModels.Transaction.TransactionListViewModel>(It.IsAny<object>()))
                .Returns(new FinansoApp.ViewModels.Transaction.TransactionListViewModel
                {
                    CurrentPage = 1,
                    PagesCount = 1,
                    Transactions = new List<ViewModels.Transaction.TransactionViewModel>
                {
                    new ViewModels.Transaction.TransactionViewModel { Amount = 100, GroupName = "Group Name", BalanceName = "Balance name", TransactionType = "Income", TransactionStatus = "Planned", TransactionDate = DateTime.Now, CurrencyId = 1, CurrencyCode = "USD", CurrencyName = "Dolar", Description = String.Empty }
                }
                });


            // Create controller
            FinansoApp.Controllers.TransactionController controller = new FinansoApp.Controllers.TransactionController(_transactionQueryRepositoryMock.Object, _transactionManagementRepositoryMock.Object, _transactionMetaQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _mapperMock.Object, _settingsQueryRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            Microsoft.AspNetCore.Mvc.IActionResult result = await controller.Index(1);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            ViewModels.Transaction.TransactionListViewModel model = Assert.IsAssignableFrom<FinansoApp.ViewModels.Transaction.TransactionListViewModel>(viewResult.ViewData.Model);
            Assert.Equal(1, model.CurrentPage);
            Assert.Equal(1, model.PagesCount);
            Assert.Single(model.Transactions);
        }

        #region HTTPGET AddTransaction

        [Fact]
        public async Task AddTransaction_GET_ShouldBeAuthorized()
        {
            // Arrange 
            MethodInfo httpGetMethod = typeof(TransactionController).GetMethod(nameof(TransactionController.AddTransaction), new[] { typeof(int?), typeof(bool) });

            // Act
            AuthorizeAttribute? httpGetAuthorizeAttribute = httpGetMethod.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            httpGetAuthorizeAttribute.Should().NotBeNull("this method should be protected by authorization");
        }

        [Fact]
        public async Task AddTransaction_ReturnsViewResult()
        {
            // Arrange
            #region Mocking ClaimsPrincipal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            IEnumerable<GetTransactionsForUser> listOfTransactions = new List<GetTransactionsForUser>
{
    new GetTransactionsForUser { TransactionId = 1, Amount = 100, GroupId = 1, GroupName = "Group Name", BalanceId = 1, BalanceName = "Balance name", TransactionType = "Income", TransactionStatus = "Planned", TransactionDate = DateTime.Now, CurrencyId = 1, CurrencyCode = "USD", CurrencyName = "Dolar", Description = String.Empty }
};
            #endregion

            #region Mocking Repository
            IEnumerable<Tuple<int, string>> transactionTypes = new List<Tuple<int, string>> { new Tuple<int, string>(1, "Income"), new Tuple<int, string>(2, "Expense") };
            IEnumerable<Tuple<int, string>> transactionStatuses = new List<Tuple<int, string>> { new Tuple<int, string>(1, "Planned"), new Tuple<int, string>(2, "Done") };
            _transactionMetaQueryRepositoryMock.Setup(x => x.GetShortListOfAllTransactionStatuses())
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(transactionStatuses));
            _transactionMetaQueryRepositoryMock.Setup(x => x.GetShortListOfAllTransactionTypes())
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(transactionTypes));

            _balanceQueryRepositoryMock.Setup(x => x.GetShortListOfBalanceForUser(It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(new List<Tuple<int, string>> { new Tuple<int, string>(1, "Balance name") }));
            _transactionMetaQueryRepositoryMock.Setup(x => x.GetTransactionIncomeCategories())
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(new List<Tuple<int, string>> { new Tuple<int, string>(1, "Income category") }));
            _transactionMetaQueryRepositoryMock.Setup(x => x.GetTransactionExpenseCategories())
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(new List<Tuple<int, string>> { new Tuple<int, string>(1, "Expense category") }));

            #endregion

            // Create controller
            FinansoApp.Controllers.TransactionController controller = new FinansoApp.Controllers.TransactionController(_transactionQueryRepositoryMock.Object, _transactionManagementRepositoryMock.Object, _transactionMetaQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _mapperMock.Object, _settingsQueryRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };


            // Act
            IActionResult result = await controller.AddTransaction(null, false);


            // Assert
            result.Should().NotBeNull();

            ViewResult viewResult = (ViewResult)result;
            viewResult.Model.Should().BeOfType<AddTransactionViewModel>();

            AddTransactionViewModel addTransactionViewModel = (AddTransactionViewModel)viewResult.Model;
            addTransactionViewModel.Error.IsError().Should().BeFalse();
        }

        #endregion

        #region HTTPPOST AddTransaction

        [Fact]
        public async Task AddTransaction_POST_ShouldBeAuthorized()
        {
            // Arrange 
            MethodInfo? httpPostMethod = typeof(TransactionController).GetMethod(nameof(TransactionController.AddTransaction), new[] { typeof(AddTransactionViewModel) });

            // Act
            AuthorizeAttribute? httpPostAuthorizeAttribute = httpPostMethod.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            httpPostAuthorizeAttribute.Should().NotBeNull("this method should be protected by authorization");
        }

        [Fact]
        public async Task AddTransaction_ShouldCallAddTransactionMethodWhenIsRecurringIsFalse()
        {
            // Arrange
            #region Mocking ClaimsPrincipal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            IEnumerable<GetTransactionsForUser> listOfTransactions = new List<GetTransactionsForUser>
{
    new GetTransactionsForUser { TransactionId = 1, Amount = 100, GroupId = 1, GroupName = "Group Name", BalanceId = 1, BalanceName = "Balance name", TransactionType = "Income", TransactionStatus = "Planned", TransactionDate = DateTime.Now, CurrencyId = 1, CurrencyCode = "USD", CurrencyName = "Dolar", Description = String.Empty }
};
            #endregion

            #region Mocking Repository

            _transactionManagementRepositoryMock.Setup(x => x.AddTransaction(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                ))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));

            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool?>.Success(true));
            #endregion

            // Create controller
            FinansoApp.Controllers.TransactionController controller = new FinansoApp.Controllers.TransactionController(_transactionQueryRepositoryMock.Object, _transactionManagementRepositoryMock.Object, _transactionMetaQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _mapperMock.Object, _settingsQueryRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            AddTransactionViewModel addTransactionViewModelInput = new AddTransactionViewModel
            {
                BalanceId = 1,
                Amount = 100,
                Description = "Description",
                TransactionDate = DateTime.Now,
                TransactionStatusId = 1,
                TransactionTypeId = 1,
                IsRecurring = false
            };

            // Act
            IActionResult result = await controller.AddTransaction(addTransactionViewModelInput);


            // Assert
            _transactionManagementRepositoryMock.Verify(mock => mock.AddTransaction(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                ), Times.Once);
            _transactionManagementRepositoryMock.Verify(mock => mock.AddTransactionMonthlyRecurring(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Never);
            _transactionManagementRepositoryMock.Verify(mock => mock.AddTransactionWeeklyRecurring(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Never);
        }


        public async Task AddTransaction_ShouldCallAddTransactionMonthlyRecurring()
        {
            // Arrange
            #region Mocking ClaimsPrincipal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            IEnumerable<GetTransactionsForUser> listOfTransactions = new List<GetTransactionsForUser>
{
    new GetTransactionsForUser { TransactionId = 1, Amount = 100, GroupId = 1, GroupName = "Group Name", BalanceId = 1, BalanceName = "Balance name", TransactionType = "Income", TransactionStatus = "Planned", TransactionDate = DateTime.Now, CurrencyId = 1, CurrencyCode = "USD", CurrencyName = "Dolar", Description = String.Empty }
};
            #endregion

            #region Mocking Repository

            _transactionManagementRepositoryMock.Setup(x => x.AddTransaction(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                ))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));

            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool?>.Success(true));
            #endregion

            // Create controller
            FinansoApp.Controllers.TransactionController controller = new FinansoApp.Controllers.TransactionController(_transactionQueryRepositoryMock.Object, _transactionManagementRepositoryMock.Object, _transactionMetaQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _mapperMock.Object, _settingsQueryRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            AddTransactionViewModel addTransactionViewModelInput = new AddTransactionViewModel
            {
                BalanceId = 1,
                Amount = 100,
                Description = "Description",
                TransactionDate = DateTime.Now,
                TransactionStatusId = 1,
                TransactionTypeId = 1,
                IsRecurring = true,
                RecurringEndDate = DateTime.Now.AddYears(1),
                RecurringStartDate = DateTime.Now,
                RecurringType = "Monthly"
            };

            // Act
            IActionResult result = await controller.AddTransaction(addTransactionViewModelInput);


            // Assert
            _transactionManagementRepositoryMock.Verify(mock => mock.AddTransaction(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                ), Times.Never);
            _transactionManagementRepositoryMock.Verify(mock => mock.AddTransactionMonthlyRecurring(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Once);
            _transactionManagementRepositoryMock.Verify(mock => mock.AddTransactionWeeklyRecurring(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Never);
        }


        public async Task AddTransaction_ShouldCallAddTransactionWeeklyRecurring()
        {
            // Arrange
            #region Mocking ClaimsPrincipal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            IEnumerable<GetTransactionsForUser> listOfTransactions = new List<GetTransactionsForUser>
{
    new GetTransactionsForUser { TransactionId = 1, Amount = 100, GroupId = 1, GroupName = "Group Name", BalanceId = 1, BalanceName = "Balance name", TransactionType = "Income", TransactionStatus = "Planned", TransactionDate = DateTime.Now, CurrencyId = 1, CurrencyCode = "USD", CurrencyName = "Dolar", Description = String.Empty }
};
            #endregion

            #region Mocking Repository

            _transactionManagementRepositoryMock.Setup(x => x.AddTransaction(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                ))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));

            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool?>.Success(true));
            #endregion

            // Create controller
            FinansoApp.Controllers.TransactionController controller = new FinansoApp.Controllers.TransactionController(_transactionQueryRepositoryMock.Object, _transactionManagementRepositoryMock.Object, _transactionMetaQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _mapperMock.Object, _settingsQueryRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            AddTransactionViewModel addTransactionViewModelInput = new AddTransactionViewModel
            {
                BalanceId = 1,
                Amount = 100,
                Description = "Description",
                TransactionDate = DateTime.Now,
                TransactionStatusId = 1,
                TransactionTypeId = 1,
                IsRecurring = true,
                RecurringEndDate = DateTime.Now.AddYears(1),
                RecurringStartDate = DateTime.Now,
                RecurringType = "Weekly"
            };

            // Act
            IActionResult result = await controller.AddTransaction(addTransactionViewModelInput);


            // Assert
            _transactionManagementRepositoryMock.Verify(mock => mock.AddTransaction(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                ), Times.Never);
            _transactionManagementRepositoryMock.Verify(mock => mock.AddTransactionMonthlyRecurring(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Never);
            _transactionManagementRepositoryMock.Verify(mock => mock.AddTransactionWeeklyRecurring(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task AddTransaction_ReturnsRedirectToActionResult_WhenActionIsSuccess()
        {
            // Arrange
            #region Mocking ClaimsPrincipal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            IEnumerable<GetTransactionsForUser> listOfTransactions = new List<GetTransactionsForUser>
{
    new GetTransactionsForUser { TransactionId = 1, Amount = 100, GroupId = 1, GroupName = "Group Name", BalanceId = 1, BalanceName = "Balance name", TransactionType = "Income", TransactionStatus = "Planned", TransactionDate = DateTime.Now, CurrencyId = 1, CurrencyCode = "USD", CurrencyName = "Dolar", Description = String.Empty }
};
            #endregion

            #region Mocking Repository

            _transactionManagementRepositoryMock.Setup(x => x.AddTransaction(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                ))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));

            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool?>.Success(true));
            #endregion

            // Create controller
            FinansoApp.Controllers.TransactionController controller = new FinansoApp.Controllers.TransactionController(_transactionQueryRepositoryMock.Object, _transactionManagementRepositoryMock.Object, _transactionMetaQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _mapperMock.Object, _settingsQueryRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            AddTransactionViewModel addTransactionViewModelInput = new AddTransactionViewModel
            {
                BalanceId = 1,
                Amount = 100,
                Description = "Description",
                TransactionDate = DateTime.Now,
                TransactionStatusId = 1,
                TransactionTypeId = 1
            };

            // Act
            IActionResult result = await controller.AddTransaction(addTransactionViewModelInput);


            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectToActionResult>();

            RedirectToActionResult redirectToActionResult = (RedirectToActionResult)result;

            redirectToActionResult.ActionName.Should().Be("Index");
        }


        [Fact]
        public async Task AddTransaction_ReturnsUnAuthorized_WhenUserDoesNottHaveAccessToBalance()
        {
            // Arrange
            #region Mocking ClaimsPrincipal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            IEnumerable<GetTransactionsForUser> listOfTransactions = new List<GetTransactionsForUser>
{
    new GetTransactionsForUser { TransactionId = 1, Amount = 100, GroupId = 1, GroupName = "Group Name", BalanceId = 1, BalanceName = "Balance name", TransactionType = "Income", TransactionStatus = "Planned", TransactionDate = DateTime.Now, CurrencyId = 1, CurrencyCode = "USD", CurrencyName = "Dolar", Description = String.Empty }
};
            #endregion

            #region Mocking Repository


            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool?>.Success(false));
            #endregion

            // Create controller
            FinansoApp.Controllers.TransactionController controller = new FinansoApp.Controllers.TransactionController(_transactionQueryRepositoryMock.Object, _transactionManagementRepositoryMock.Object, _transactionMetaQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _mapperMock.Object, _settingsQueryRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            AddTransactionViewModel addTransactionViewModelInput = new AddTransactionViewModel
            {
                BalanceId = 1,
                Amount = 100,
                Description = "Description",
                TransactionDate = DateTime.Now,
                TransactionStatusId = 1,
                TransactionTypeId = 1
            };

            // Act
            IActionResult result = await controller.AddTransaction(addTransactionViewModelInput);


            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task AddTransaction_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            #region Mocking ClaimsPrincipal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            IEnumerable<GetTransactionsForUser> listOfTransactions = new List<GetTransactionsForUser>
{
    new GetTransactionsForUser { TransactionId = 1, Amount = 100, GroupId = 1, GroupName = "Group Name", BalanceId = 1, BalanceName = "Balance name", TransactionType = "Income", TransactionStatus = "Planned", TransactionDate = DateTime.Now, CurrencyId = 1, CurrencyCode = "USD", CurrencyName = "Dolar", Description = String.Empty }
};
            #endregion

            #region Mocking Repository
            IEnumerable<Tuple<int, string>> transactionTypes = new List<Tuple<int, string>> { new Tuple<int, string>(1, "Income"), new Tuple<int, string>(2, "Expense") };
            IEnumerable<Tuple<int, string>> transactionStatuses = new List<Tuple<int, string>> { new Tuple<int, string>(1, "Planned"), new Tuple<int, string>(2, "Done") };
            _transactionMetaQueryRepositoryMock.Setup(x => x.GetShortListOfAllTransactionStatuses())
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(transactionStatuses));
            _transactionMetaQueryRepositoryMock.Setup(x => x.GetShortListOfAllTransactionTypes())
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(transactionTypes));

            _balanceQueryRepositoryMock.Setup(x => x.GetShortListOfBalanceForUser(It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(new List<Tuple<int, string>> { new Tuple<int, string>(1, "Balance name") }));

            _transactionMetaQueryRepositoryMock.Setup(x => x.GetTransactionIncomeCategories())
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(new List<Tuple<int, string>> { new Tuple<int, string>(1, "Income category") }));
            _transactionMetaQueryRepositoryMock.Setup(x => x.GetTransactionExpenseCategories())
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(new List<Tuple<int, string>> { new Tuple<int, string>(1, "Expense category") }));

            _transactionManagementRepositoryMock.Setup(x => x.AddTransaction(
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()
                ))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));

            _balanceQueryRepositoryMock.Setup(x => x.HasUserAccessToBalance(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool?>.Success(true));
            #endregion

            // Create controller
            FinansoApp.Controllers.TransactionController controller = new FinansoApp.Controllers.TransactionController(_transactionQueryRepositoryMock.Object, _transactionManagementRepositoryMock.Object, _transactionMetaQueryRepositoryMock.Object, _balanceQueryRepositoryMock.Object, _mapperMock.Object, _settingsQueryRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };
            controller.ModelState.AddModelError("Amount", "Amount is required");

            AddTransactionViewModel addTransactionViewModelInput = new AddTransactionViewModel
            {
                BalanceId = 1,
                Amount = 100,
                Description = "Description",
                TransactionDate = DateTime.Now,
                TransactionStatusId = 1,
                TransactionTypeId = 1,
                TransactionIncomeCategory = 1,
                TransactionExpenseCategoryId = 1
            };

            // Act
            IActionResult result = await controller.AddTransaction(addTransactionViewModelInput);


            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();

            ViewResult viewResult = (ViewResult)result;
            viewResult.Model.Should().BeOfType<AddTransactionViewModel>();
        }

        #endregion
    }
}
