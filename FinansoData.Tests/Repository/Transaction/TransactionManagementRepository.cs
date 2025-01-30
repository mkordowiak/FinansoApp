using FinansoData.Data;
using FinansoData.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Tests.Repository.Transaction
{
    public class TransactionManagementRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly List<Models.Currency> _currencies;
        private readonly List<TransactionStatus> _transactionStatuses;
        private readonly List<TransactionType> _transactionTypes;
        private readonly List<AppUser> _appUsers;
        private readonly List<Models.Group> _groups;
        private readonly List<Models.Balance> _balances;

        public TransactionManagementRepository()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

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

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureCreated();
                context.Currencies.AddRange(_currencies);
                context.TransactionStatuses.AddRange(_transactionStatuses);
                context.TransactionTypes.AddRange(_transactionTypes);
                context.Users.AddRange(_appUsers);
                context.Groups.AddRange(_groups);
                context.Balances.AddRange(_balances);
                context.SaveChanges();
            }


        }

        #region AddTransaction

        [Fact]
        public async Task AddTransaction_WhenCalled_ReturnsTrue()
        {
            // Arrange
            RepositoryResult<bool> result;
            BalanceTransaction? inMemoryDbTransaction;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                FinansoData.Repository.Transaction.TransactionManagementRepository repository = new FinansoData.Repository.Transaction.TransactionManagementRepository(context);

                // Act
                result = await repository.AddTransaction(100m, "desc", 1, DateTime.Now, "1", 1, 1, 1);
                inMemoryDbTransaction = context.BalanceTransactions.FirstOrDefault();
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            inMemoryDbTransaction.Should().NotBeNull();
        }

        [Fact]
        public async Task AddTransaction_ShouldReturnFailureWhenBalanceIdIsWrong()
        {
            // Arrange
            RepositoryResult<bool> result;
            BalanceTransaction? inMemoryDbTransaction;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                FinansoData.Repository.Transaction.TransactionManagementRepository repository = new FinansoData.Repository.Transaction.TransactionManagementRepository(context);

                // Act
                result = await repository.AddTransaction(100m, "desc", 999, DateTime.Now, "1", 1, 1, 1);
                inMemoryDbTransaction = context.BalanceTransactions.FirstOrDefault();
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeFalse();
            inMemoryDbTransaction.Should().BeNull();
        }

        [Fact]
        public async Task AddTransaction_ShouldReturnFailureWhenUserNameIsWrong()
        {
            // Arrange
            RepositoryResult<bool> result;
            BalanceTransaction? inMemoryDbTransaction;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                FinansoData.Repository.Transaction.TransactionManagementRepository repository = new FinansoData.Repository.Transaction.TransactionManagementRepository(context);

                // Act
                result = await repository.AddTransaction(100m, "desc", 1, DateTime.Now, "Wrong username 123", 1, 1, 1);
                inMemoryDbTransaction = context.BalanceTransactions.FirstOrDefault();
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeFalse();
            inMemoryDbTransaction.Should().BeNull();
        }

        #endregion
    }
}
