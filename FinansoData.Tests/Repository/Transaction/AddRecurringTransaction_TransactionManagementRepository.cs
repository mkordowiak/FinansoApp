using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Balance;
using FinansoData.Repository.Transaction;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Tests.Repository.Transaction
{
    public class AddRecurringTransaction_TransactionManagementRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<IBalanceManagementRepository> _balanceManagementRepositoryMock;
        private readonly List<Models.Currency> _currencies;
        private readonly List<TransactionStatus> _transactionStatuses;
        private readonly List<TransactionType> _transactionTypes;
        private readonly List<AppUser> _appUsers;
        private readonly List<Models.Group> _groups;
        private readonly List<Models.Balance> _balances;

        public AddRecurringTransaction_TransactionManagementRepository()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _balanceManagementRepositoryMock = new Mock<IBalanceManagementRepository>();

            _currencies = new List<Models.Currency>
            {
                new Models.Currency { Id = 1, Name = "Euro", Code = "EUR" },
                new Models.Currency { Id = 2, Name = "US Dollar", Code = "USD" }
            };

            _transactionStatuses = new List<TransactionStatus>
            {
                new TransactionStatus { Id = 1, Name = "Planned" },
                new TransactionStatus { Id = 2, Name = "Done" },
                new TransactionStatus { Id = 3, Name = "Canceled" }
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
                new Models.Balance { Id = 1, Name = "Balance 1", Group = _groups[0], Amount = 10 },
                new Models.Balance { Id = 2, Name = "Balance 2", Group = _groups[0], Amount = 999 }
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

        [Fact]
        public async Task AddTransactionMonthlyRecurring_WhenCalled_ReturnTrue()
        {
            // Arrange
            RepositoryResult<bool> result;
            List<BalanceTransaction> inMemoryDbTransaction;

            using(ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionManagementRepository transactionManagementRepository = new TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);
                // Act
                result = await transactionManagementRepository.AddTransactionMonthlyRecurring(100m, "description", 1, new DateTime(2025, 1, 1), new DateTime(2026, 1, 1), _appUsers[0].UserName, 1, 1);
                inMemoryDbTransaction = await context.BalanceTransactions.ToListAsync();
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            inMemoryDbTransaction.Should().NotBeNull();

            inMemoryDbTransaction.Count.Should().Be(13);
        }

        [Fact]
        public async Task AddTransactionWeeklyRecurring_WhenCalled_ReturnTrue()
        {
            // Arrange
            RepositoryResult<bool> result;
            List<BalanceTransaction> inMemoryDbTransaction;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionManagementRepository transactionManagementRepository = new TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);
                // Act
                result = await transactionManagementRepository.AddTransactionWeeklyRecurring(100m, "description", 1, new DateTime(2025, 1, 1), new DateTime(2025, 06, 1), _appUsers[0].UserName, 1, 1);
                inMemoryDbTransaction = await context.BalanceTransactions.ToListAsync();
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            inMemoryDbTransaction.Should().NotBeNull();
            inMemoryDbTransaction.Should().NotBeEmpty();

            inMemoryDbTransaction.Count.Should().Be(22);
        }


        [Fact]
        public async Task AddTransactionMonthlyRecurring_ShouldAddRecuringTransactionId()
        {
            // Arrange
            RepositoryResult<bool> result;
            List<BalanceTransaction> inMemoryDbTransaction;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionManagementRepository transactionManagementRepository = new TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);
                // Act
                result = await transactionManagementRepository.AddTransactionMonthlyRecurring(100m, "description", 1, new DateTime(2025, 1, 1), new DateTime(2026, 1, 1), _appUsers[0].UserName, 1, 1);
                inMemoryDbTransaction = await context.BalanceTransactions.ToListAsync();
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            inMemoryDbTransaction.Should().NotBeNull();
            inMemoryDbTransaction.Should().NotBeEmpty();

            // Check if all transactions have the same RecurringTransactionId
            inMemoryDbTransaction.All(x => x.RecurringTransactionId == inMemoryDbTransaction[0].RecurringTransactionId).Should().BeTrue();
        }

        [Fact]
        public async Task AddTransactionMonthlyRecurring_ShouldReturnFailureWhenBalanceIdIsWrong()
        {
            // Arrange
            RepositoryResult<bool> result;
            List<BalanceTransaction> inMemoryDbTransaction;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionManagementRepository transactionManagementRepository = new TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);
                // Act
                result = await transactionManagementRepository.AddTransactionMonthlyRecurring(100m, "description", 999, new DateTime(2025, 1, 1), new DateTime(2026, 1, 1), _appUsers[0].UserName, 1, 1);
                inMemoryDbTransaction = await context.BalanceTransactions.ToListAsync();
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeFalse();
            inMemoryDbTransaction.Should().NotBeNull();
            inMemoryDbTransaction.Should().BeEmpty();
        }

        [Fact]
        public async Task AddTransactionMonthlyRecurring_ShouldReturnFailureWhenUserNameIsWrong()
        {
            // Arrange
            RepositoryResult<bool> result;
            List<BalanceTransaction> inMemoryDbTransaction;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionManagementRepository transactionManagementRepository = new TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);
                // Act
                result = await transactionManagementRepository.AddTransactionMonthlyRecurring(100m, "description", 1, new DateTime(2025, 1, 1), new DateTime(2026, 1, 1), "aaaa", 1, 1);
                inMemoryDbTransaction = await context.BalanceTransactions.ToListAsync();
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeFalse();
            inMemoryDbTransaction.Should().NotBeNull();
            inMemoryDbTransaction.Should().BeEmpty();
        }
    }
}
