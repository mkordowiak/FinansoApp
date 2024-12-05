using FinansoData.Data;
using FinansoData.Repository.Currency;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Tests.Repository.Currency
{

    public class CurrencyQueryRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private List<FinansoData.Models.Currency> _currencies;

        public CurrencyQueryRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                _currencies = new List<FinansoData.Models.Currency>();

                _currencies.Add(new Models.Currency { Id = 1, Name = "PLN" });
                _currencies.Add(new Models.Currency { Id = 2, Name = "USD" });
                _currencies.Add(new Models.Currency { Id = 3, Name = "EUR" });

                context.Currencies.AddRange(_currencies);

                context.SaveChanges();
            }
        }

        #region GetCurrencyById
        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyById_ShouldReturnValue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context);
                FinansoData.Models.Currency expectedCurrency = _currencies.First();

                // Act
                RepositoryResult<DataViewModel.Currency.CurrencyViewModel> result = await currencyQueryRepository.GetCurrencyById(expectedCurrency.Id);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Id.Should().Be(expectedCurrency.Id);
                result.Value.Name.Should().Be(expectedCurrency.Name);
            }
        }

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyById_ShouldReturnNullWhenWrongId()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context);
                int wrongId = 999;

                // Act
                RepositoryResult<DataViewModel.Currency.CurrencyViewModel> result = await currencyQueryRepository.GetCurrencyById(wrongId);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeNull();
            }
        }

        #endregion

        #region GetCurrencyModelById

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyModelById_ShouldReturnValue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context);
                FinansoData.Models.Currency expectedCurrency = _currencies.First();

                // Act
                RepositoryResult<Models.Currency?> result = await currencyQueryRepository.GetCurrencyModelById(expectedCurrency.Id);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Id.Should().Be(expectedCurrency.Id);
                result.Value.Name.Should().Be(expectedCurrency.Name);
            }
        }

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyModelById_ShouldReturnNullWhenWrongId()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context);
                int wrongId = 999;

                // Act
                RepositoryResult<Models.Currency?> result = await currencyQueryRepository.GetCurrencyModelById(wrongId);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeNull();
            }
        }

        #endregion

        #region GetAllCurrencies

        [Fact]
        public async Task CurrencyQueryRepository_GetAllCurrencies_ShouldReturnAllCurrencies()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context);

                // Act
                RepositoryResult<IEnumerable<DataViewModel.Currency.CurrencyViewModel>> result = await currencyQueryRepository.GetAllCurrencies();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().HaveCount(_currencies.Count);
            }
        }

        #endregion

    }
}
