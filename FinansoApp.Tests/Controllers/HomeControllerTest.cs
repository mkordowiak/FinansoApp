using FinansoApp.Controllers;
using FinansoData;
using FinansoData.Repository.Chart;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace FinansoApp.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<IChartDataRepository> _mockChartDataRepository;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockChartDataRepository = new Mock<IChartDataRepository>();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithChartsInViewData()
        {
            // Arrange
            List<Tuple<string, decimal>> incomeData = new List<Tuple<string, decimal>>()
            {
                new Tuple<string, decimal>("Salary", 5000),
                new Tuple<string, decimal>("Investments", 2000)
            };
            List<Tuple<string, decimal>> expenseData = new List<Tuple<string, decimal>>()
            {
                new Tuple<string, decimal>("Rent", 1500),
                new Tuple<string, decimal>("Groceries", 500)
            };

            _mockChartDataRepository.Setup(x => x.GetIncomesInCategories(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<string, decimal>>>.Success(incomeData));
            _mockChartDataRepository.Setup(x => x.GetExpensesInCategories(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<string, decimal>>>.Success(expenseData));

            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            HomeController controller = new HomeController(_mockLogger.Object, _mockChartDataRepository.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };


            // Act
            IActionResult result = await controller.Index();

            // Assert
            ViewResult? viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewData["ExpenseVerticalChart"].Should().NotBeNull();
            viewResult.ViewData["IncomeVerticalChart"].Should().NotBeNull();
        }

        [Fact]
        public async Task Index_ReturnsErrorView_WhenDataFetchFails()
        {
            // Arrange
            _mockChartDataRepository.Setup(repo => repo.GetIncomesInCategories(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<string, decimal>>>.Failure("Error", FinansoData.ErrorType.ServerError));
            _mockChartDataRepository.Setup(repo => repo.GetExpensesInCategories(It.IsAny<string>(), It.IsAny<int>()))
               .ReturnsAsync(RepositoryResult<IEnumerable<Tuple<string, decimal>>>.Failure("Error", FinansoData.ErrorType.ServerError));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            HomeController controller = new HomeController(_mockLogger.Object, _mockChartDataRepository.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };


            // Act
            IActionResult result = await controller.Index();

            // Assert
            ViewResult? viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Be("Error");
        }
    }
}
