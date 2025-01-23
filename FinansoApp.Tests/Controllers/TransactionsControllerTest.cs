using AutoMapper;
using FinansoData.DataViewModel.Transaction;
using FinansoData;
using FinansoData.Repository.Settings;
using FinansoData.Repository.Transaction;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Tests.Controllers
{
    public class TransactionsControllerTest
    {
        private Mock<ITransactionsQueryRepository> _transactionQueryRepositoryMock;
        private Mock<ITransactionManagementRepository> _transactionManagementRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ISettingsQueryRepository> _settingsQueryRepositoryMock;

        public TransactionsControllerTest()
        {
            _transactionManagementRepositoryMock = new Mock<ITransactionManagementRepository>();
            _transactionQueryRepositoryMock = new Mock<ITransactionsQueryRepository>();
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

            IEnumerable < GetTransactionsForUser > listOfTransactions = new List<GetTransactionsForUser>
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
            FinansoApp.Controllers.TransactionController controller = new FinansoApp.Controllers.TransactionController(_transactionQueryRepositoryMock.Object, _transactionManagementRepositoryMock.Object, _mapperMock.Object, _settingsQueryRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };
            
            // Act
            Microsoft.AspNetCore.Mvc.IActionResult result = await controller.Index(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            ViewModels.Transaction.TransactionListViewModel model = Assert.IsAssignableFrom<FinansoApp.ViewModels.Transaction.TransactionListViewModel>(viewResult.ViewData.Model);
            Assert.Equal(1, model.CurrentPage);
            Assert.Equal(1, model.PagesCount);
            Assert.Single(model.Transactions);
        }
    }
}
