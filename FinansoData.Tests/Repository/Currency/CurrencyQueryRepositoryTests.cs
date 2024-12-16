using FinansoData.Data;
using FinansoData.DataViewModel.Currency;
using FinansoData.Repository;
using FinansoData.Repository.Currency;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoData.Tests.Repository.Currency
{

    public class CurrencyQueryRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ICacheWrapper> _cacheWrapperMock;
        private List<FinansoData.Models.Currency> _currencies;


        public CurrencyQueryRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;


            _cacheWrapperMock = new Mock<ICacheWrapper>();

            _cacheWrapperMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()));

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                _currencies = new List<FinansoData.Models.Currency>();

                _currencies.Add(new Models.Currency { Id = 1, Code = "PLN", Name = "Polski zloty" });
                _currencies.Add(new Models.Currency { Id = 2, Code = "USD", Name = "Dolar amerykanski" });
                _currencies.Add(new Models.Currency { Id = 3, Code = "EUR", Name = "Euro" });

                context.Currencies.AddRange(_currencies);

                context.SaveChanges();
            }
        }

        #region GetCurrencyById
        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyById_ShouldReturnValueFromDB()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Mock<ICacheWrapper> cacheWrapperMock = new Mock<ICacheWrapper>();

                CurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);
                FinansoData.Models.Currency expectedCurrency = _currencies.First();

                // Act
                RepositoryResult<DataViewModel.Currency.CurrencyViewModel> result = await currencyQueryRepository.GetCurrencyById(expectedCurrency.Id);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Id.Should().Be(expectedCurrency.Id);
                result.Value.Name.Should().Be(expectedCurrency.Name);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyById_ShouldSaveCache()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Mock<ICacheWrapper> cacheWrapperMock = new Mock<ICacheWrapper>();

                CurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);
                FinansoData.Models.Currency expectedCurrency = _currencies.First();

                // Act
                RepositoryResult<DataViewModel.Currency.CurrencyViewModel> result = await currencyQueryRepository.GetCurrencyById(expectedCurrency.Id);

                // Assert
                result.IsSuccess.Should().BeTrue();
                _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Once);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyById_ShouldReturnValueFromCache()
        {
            // Arrange
            CurrencyViewModel? outCurrencyVM = new CurrencyViewModel { Id = _currencies.First().Id + 1, Name = _currencies.First().Name };
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out outCurrencyVM))
                .Returns(true);
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Mock<ICacheWrapper> cacheWrapperMock = new Mock<ICacheWrapper>();

                CurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);
                
                // Act
                RepositoryResult<DataViewModel.Currency.CurrencyViewModel> result = await currencyQueryRepository.GetCurrencyById(1);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeEquivalentTo(outCurrencyVM);
                _cacheWrapperMock.Verify(x => x.TryGetValue(It.IsAny<string>(), out outCurrencyVM), Times.Once);


                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyById_ShouldReturnNullWhenWrongId()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);
                int wrongId = 999;

                // Act
                RepositoryResult<DataViewModel.Currency.CurrencyViewModel> result = await currencyQueryRepository.GetCurrencyById(wrongId);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeNull();

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        #endregion

        #region GetCurrencyModelById

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyModelById_ShouldReturnValueFromDB()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);
                FinansoData.Models.Currency expectedCurrency = _currencies.First();

                // Act
                RepositoryResult<Models.Currency?> result = await currencyQueryRepository.GetCurrencyModelById(expectedCurrency.Id);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Id.Should().Be(expectedCurrency.Id);
                result.Value.Name.Should().Be(expectedCurrency.Name);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyModelById_ShouldSaveCache()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);
                FinansoData.Models.Currency expectedCurrency = _currencies.First();

                // Act
                RepositoryResult<Models.Currency?> result = await currencyQueryRepository.GetCurrencyModelById(expectedCurrency.Id);

                // Assert
                result.IsSuccess.Should().BeTrue();
                _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Once);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyModelById_ShouldReturnValueFromCache()
        {
            // Arrange
            Models.Currency? outCurrencyVM = new Models.Currency { Id = _currencies.First().Id+1, Name = _currencies.First().Name };
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out outCurrencyVM))
                .Returns(true);
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);
                

                // Act
                RepositoryResult<Models.Currency?> result = await currencyQueryRepository.GetCurrencyModelById(1);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeEquivalentTo(outCurrencyVM);
                _cacheWrapperMock.Verify(x => x.TryGetValue(It.IsAny<string>(), out outCurrencyVM), Times.Once);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CurrencyQueryRepository_GetCurrencyModelById_ShouldReturnNullWhenWrongId()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);
                int wrongId = 999;

                // Act
                RepositoryResult<Models.Currency?> result = await currencyQueryRepository.GetCurrencyModelById(wrongId);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeNull();

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        #endregion

        #region GetAllCurrencies

        [Fact]
        public async Task CurrencyQueryRepository_GetAllCurrencies_ShouldReturnAllCurrenciesFromDatabase()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);

                // Act
                RepositoryResult<IEnumerable<DataViewModel.Currency.CurrencyViewModel>> result = await currencyQueryRepository.GetAllCurrencies();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().HaveCount(_currencies.Count);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task CurrencyQueryRepository_GetAllCurrencies_ShouldSetCache()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);

                // Act
                RepositoryResult<IEnumerable<DataViewModel.Currency.CurrencyViewModel>> result = await currencyQueryRepository.GetAllCurrencies();

                // Assert
                result.IsSuccess.Should().BeTrue();
                _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()));

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }


        [Fact]
        public async Task CurrencyQueryRepository_GetAllCurrencies_ShouldReturnAllCurrenciesFromCache()
        {
            // Arrange
            IEnumerable<CurrencyViewModel> outCurrencyVM = _currencies.Select(x => new CurrencyViewModel { Id = x.Id, Name = x.Name });

            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out outCurrencyVM))
                .Returns(true);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                ICurrencyQueryRepository currencyQueryRepository = new CurrencyQueryRepository(context, _cacheWrapperMock.Object);

                // Act
                RepositoryResult<IEnumerable<DataViewModel.Currency.CurrencyViewModel>> result = await currencyQueryRepository.GetAllCurrencies();

                // Assert
                result.IsSuccess.Should().BeTrue();
                _cacheWrapperMock.Verify(x => x.TryGetValue(It.IsAny<string>(), out outCurrencyVM), Times.Once);
                result.Value.Should().HaveCount(outCurrencyVM.Count());
                result.Value.Should().BeEquivalentTo(outCurrencyVM);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }



        #endregion

    }
}
