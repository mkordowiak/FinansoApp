using FinansoData.Data;
using FinansoData.DataViewModel.Transaction;
using FinansoData.Models;
using FinansoData.Repository;
using FinansoData.Repository.Transaction;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoData.Tests.Repository.Transaction
{
    public class TransactionQueryRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ICacheWrapper> _cacheWrapperMock;
        private List<FinansoData.Models.Currency> _currencies;
        private List<FinansoData.Models.TransactionStatus> _transactionStatuses;
        private List<FinansoData.Models.TransactionType> _transactionTypes;
        private List<FinansoData.Models.BalanceTransaction> _balanceTransactions;
        private List<FinansoData.Models.Balance> _balances;
        private List<FinansoData.Models.AppUser> _appUsers;
        private List<FinansoData.Models.Group> _groups;

        public TransactionQueryRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _cacheWrapperMock = new Mock<ICacheWrapper>();

            _currencies = new List<FinansoData.Models.Currency>
            {
                new FinansoData.Models.Currency { Id = 1, Name = "Dolar", Code = "USD", ExchangeRate = 1 },
                new FinansoData.Models.Currency { Id = 2, Name = "Euro", Code= "EUR", ExchangeRate = 1.2m },
                new FinansoData.Models.Currency { Id = 3, Name = "Pound", Code="GBP", ExchangeRate = 1.2m }
            };

            _transactionStatuses = new List<FinansoData.Models.TransactionStatus>
            {
                new FinansoData.Models.TransactionStatus { Id = 1, Name = "Planded" },
                new FinansoData.Models.TransactionStatus { Id = 2, Name = "Completed" },
                new FinansoData.Models.TransactionStatus { Id = 3, Name = "Canceled" }
            };

            _transactionTypes = new List<FinansoData.Models.TransactionType>
            {
                new FinansoData.Models.TransactionType { Id = 1, Name = "Income" },
                new FinansoData.Models.TransactionType { Id = 2, Name = "Expense" }
            };

            _appUsers = new List<AppUser>
            {
                new AppUser { Id = "1", UserName = "1", NormalizedUserName = "NormalizedUser1", NormalizedEmail = "NormalizedEmail1", FirstName = "1", LastName = "1" },
                new AppUser { Id = "2", UserName = "2", NormalizedUserName = "NormalizedUser2", NormalizedEmail = "NormalizedEmail2", FirstName = "2", LastName = "2" }
            };

            _groups = new List<Models.Group>
            {
                new Models.Group { Id = 1, Name = "Group 1", OwnerAppUser = _appUsers[1] }
            };

            _balances = new List<Models.Balance>
            {
                new Models.Balance { Id = 1, Name = "Balance 1", Group = _groups[0] },
                new Models.Balance { Id = 2, Name = "Balance 2", Group = _groups[0] }
            };

            _balanceTransactions = new List<FinansoData.Models.BalanceTransaction>
            {
                new FinansoData.Models.BalanceTransaction { Id = 1, Amount = 100, AppUser = _appUsers[0], Balance = _balances[0], CurrencyId = 1, Description = "Test 1", TransactionDate = DateTime.Now, TransactionStatusId = 1, TransactionTypeId = 1 },
                new FinansoData.Models.BalanceTransaction { Id = 2, Amount = 200, AppUser = _appUsers[0], Balance = _balances[0], CurrencyId = 1, Description = "Test 2", TransactionDate = DateTime.Now, TransactionStatusId = 1, TransactionTypeId = 1 },
                new FinansoData.Models.BalanceTransaction { Id = 3, Amount = 300, AppUser = _appUsers[0], Balance = _balances[0], CurrencyId = 1, Description = "Test 3", TransactionDate = DateTime.Now, TransactionStatusId = 1, TransactionTypeId = 1 },
                new FinansoData.Models.BalanceTransaction { Id = 4, Amount = 400, AppUser = _appUsers[1], Balance = _balances[1], CurrencyId = 2, Description = "Test 4", TransactionDate = DateTime.Now, TransactionStatusId = 2, TransactionTypeId = 2 }
            };


            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureCreated();
                context.Currencies.AddRange(_currencies);
                context.TransactionStatuses.AddRange(_transactionStatuses);
                context.TransactionTypes.AddRange(_transactionTypes);
                context.Users.AddRange(_appUsers);
                context.Groups.AddRange(_groups);
                context.Balances.AddRange(_balances);
                context.BalanceTransactions.AddRange(_balanceTransactions);
                context.SaveChanges();
            }
        }

        #region GetTransactionsForBalance
        [Fact]
        public async Task GetTransactionsForBalance_ShouldReturnTransactionsForBalance_FromDB()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetTransactionsForBalance>>.IsAny))
                .Returns(false);
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<int>.IsAny))
                .Returns(false);

            RepositoryResult<IEnumerable<GetTransactionsForBalance>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionsQueryRepository transactionsQueryRepository = new TransactionsQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionsQueryRepository.GetTransactionsForBalance(1, 1, 10);
                context.Database.EnsureDeleted();
            }

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            List<GetTransactionsForBalance> expectedTransactions = _balanceTransactions
                .Where(x => x.BalanceId == 1)
                .Select(x => new GetTransactionsForBalance
                {
                    TransactionId = x.Id,
                    Amount = x.Amount,
                    Description = x.Description,
                    TransactionDate = x.TransactionDate,
                    TransactionStatus = x.TransactionStatus.Name,
                    TransactionType = x.TransactionType.Name
                }).ToList();

            result.Value.Should().BeEquivalentTo(expectedTransactions);
            _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<IEnumerable<GetTransactionsForBalance>>(), It.IsAny<TimeSpan>()), Times.Once, "Method should save result data to cache");
            _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Once, "Method should save result count to cache");
        }

        [Fact]
        public async Task GetTransactionsForBalance_ShouldReturnTransactionsForBalance_FromCache()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetTransactionsForBalance>>.IsAny))
                .Returns(true);
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<int>.IsAny))
                .Returns(true);

            RepositoryResult<IEnumerable<GetTransactionsForBalance>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionsQueryRepository transactionsQueryRepository = new TransactionsQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionsQueryRepository.GetTransactionsForBalance(1, 1, 10);
                context.Database.EnsureDeleted();
            }

            // Assert
            result.Should().NotBeNull();
            _cacheWrapperMock.Verify(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetTransactionsForBalance>>.IsAny), Times.Once);
            _cacheWrapperMock.Verify(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<int>.IsAny), Times.Once);
            _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<IEnumerable<GetTransactionsForBalance>>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        #endregion

        #region GetTransactionsForUser

        [Fact]
        public async Task GetTransactionsForUser_ShouldReturnTransactionsForBalance_FromDB()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetTransactionsForBalance>>.IsAny))
                .Returns(false);
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<int>.IsAny))
                .Returns(false);

            RepositoryResult<IEnumerable<GetTransactionsForUser>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionsQueryRepository transactionsQueryRepository = new TransactionsQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionsQueryRepository.GetTransactionsForUser("NormalizedUser1", 1, 10);
                context.Database.EnsureDeleted();
            }

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            List<GetTransactionsForBalance> expectedTransactions = _balanceTransactions
                .Where(x => x.AppUserId == "1")
                .Select(x => new GetTransactionsForBalance
                {
                    TransactionId = x.Id,
                    Amount = x.Amount,
                    Description = x.Description,
                    TransactionDate = x.TransactionDate,
                    TransactionStatus = x.TransactionStatus.Name,
                    TransactionType = x.TransactionType.Name
                }).ToList();

            result.Value.Should().BeEquivalentTo(expectedTransactions);
            _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<IEnumerable<GetTransactionsForUser>>(), It.IsAny<TimeSpan>()), Times.Once, "Method should save result data to cache");
            _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Once, "Method should save result count to cache");
        }

        [Fact]
        public async Task GetTransactionsForUser_ShouldReturnTransactionsForBalance_FromCache()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetTransactionsForUser>>.IsAny))
                .Returns(true);
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<int>.IsAny))
                .Returns(true);

            RepositoryResult<IEnumerable<GetTransactionsForUser>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionsQueryRepository transactionsQueryRepository = new TransactionsQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionsQueryRepository.GetTransactionsForUser(string.Empty, 1, 10);
                context.Database.EnsureDeleted();
            }

            // Assert
            result.Should().NotBeNull();
            _cacheWrapperMock.Verify(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetTransactionsForUser>>.IsAny), Times.Once);
            _cacheWrapperMock.Verify(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<int>.IsAny), Times.Once);
            _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<IEnumerable<GetTransactionsForUser>>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        #endregion


    }
}
