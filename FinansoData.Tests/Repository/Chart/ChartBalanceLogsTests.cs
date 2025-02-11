using FinansoData.Data;
using FinansoData.DataViewModel.Chart;
using FinansoData.Repository;
using FinansoData.Repository.Chart;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoData.Tests.Repository.Chart
{
    public class ChartBalanceLogsTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ICacheWrapper> _cacheWrapperMock;
        private readonly List<Models.Balance> _balances;
        private readonly List<Models.BalanceLog> _balanceLogs;

        public ChartBalanceLogsTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _cacheWrapperMock = new Mock<ICacheWrapper>();

            _balances = new List<Models.Balance>
            {
                new Models.Balance { Id = 1, Name = "Balance 1", GroupId = 1, CurrencyId = 1 },
                new Models.Balance { Id = 2, Name = "Balance 2", GroupId = 1, CurrencyId = 1 }
            };

            _balanceLogs = new List<Models.BalanceLog>
            {
                new Models.BalanceLog { Id = 1, Amount = 100, BalanceId = 1, Date = DateTime.Now },
                new Models.BalanceLog { Id = 2, Amount = 200, BalanceId = 1, Date = DateTime.Now },
                new Models.BalanceLog { Id = 3, Amount = 300, BalanceId = 2, Date = DateTime.Now }
            };

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Balances.AddRange(_balances);
                context.BalanceLogs.AddRange(_balanceLogs);
                context.SaveChanges();
            }
        }

        #region 

        [Fact]
        public async Task BalanceLogsByMonth_ShouldGetDataFromDb()
        {
            // Arrange 
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);

            RepositoryResult<IEnumerable<DataViewModel.Chart.BalanceLogAverage>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ChartDataRepository chartDataRepository = new ChartDataRepository(context, _cacheWrapperMock.Object);

                // Act 
                result = await chartDataRepository.BalanceLogsByMonth(1);

                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<List<BalanceLogAverage>>();
        }

        [Fact]
        public async Task BalanceLogsByMonth_ShouldGetDataFromCache()
        {
            // Arrange 
            IEnumerable<BalanceLogAverage> expected = new List<BalanceLogAverage>
            {
                new BalanceLogAverage { Year = 2021, Month = 1, Average = 999 },
                new BalanceLogAverage { Year = 2021, Month = 2, Average = 77 }
            };

            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out expected))
                .Returns(true);

            RepositoryResult<IEnumerable<DataViewModel.Chart.BalanceLogAverage>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ChartDataRepository chartDataRepository = new ChartDataRepository(context, _cacheWrapperMock.Object);

                // Act 
                result = await chartDataRepository.BalanceLogsByMonth(1);

                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();

            result.Value.Should().BeOfType<List<BalanceLogAverage>>();

            List<BalanceLogAverage> resultValue = (List<BalanceLogAverage>)result.Value;
            resultValue.Should().BeEquivalentTo(expected);
        }

        #endregion
    }
}
