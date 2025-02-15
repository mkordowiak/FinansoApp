using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Balance;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoData.Tests.Repository.Transaction
{
    public class AddTransaction_TransactionManagementRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<IBalanceManagementRepository> _balanceManagementRepositoryMock;
        private readonly List<Models.Currency> _currencies;
        private readonly List<TransactionStatus> _transactionStatuses;
        private readonly List<TransactionType> _transactionTypes;
        private readonly List<AppUser> _appUsers;
        private readonly List<Models.Group> _groups;
        private readonly List<Models.Balance> _balances;

        public AddTransaction_TransactionManagementRepository()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _balanceManagementRepositoryMock = new Mock<IBalanceManagementRepository>();

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
        public async Task AddTransaction_WhenCalled_ReturnsTrue()
        {
            // Arrange
            RepositoryResult<bool> result;
            BalanceTransaction? inMemoryDbTransaction;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                FinansoData.Repository.Transaction.TransactionManagementRepository repository = new FinansoData.Repository.Transaction.TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);

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
                FinansoData.Repository.Transaction.TransactionManagementRepository repository = new FinansoData.Repository.Transaction.TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);

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
                FinansoData.Repository.Transaction.TransactionManagementRepository repository = new FinansoData.Repository.Transaction.TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);

                // Act
                result = await repository.AddTransaction(100m, "desc", 1, DateTime.Now, "Wrong username 123", 1, 1, 1);
                inMemoryDbTransaction = context.BalanceTransactions.FirstOrDefault();
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeFalse();
            inMemoryDbTransaction.Should().BeNull();
        }


        [Fact]
        public async Task AddTransaction_ShouldAddToBalance()
        {
            // Arrange
            RepositoryResult<bool> result;
            BalanceTransaction? inMemoryDbTransaction;
            int balanceId = 1;

            
            decimal transactionIncomeAmount = 33m;
            string userName = _appUsers[0].UserName;


            _balanceManagementRepositoryMock.Setup(x => x.AddToBalanceAmount(It.IsAny<int>(), It.IsAny<decimal>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(false));
            _balanceManagementRepositoryMock.Setup(x => x.AddToBalanceAmount(It.Is<int>(id => id == balanceId), It.Is<decimal>(amount => amount == -transactionIncomeAmount)))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));

            

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                FinansoData.Repository.Transaction.TransactionManagementRepository repository = new FinansoData.Repository.Transaction.TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);

                // Act
                result = await repository.AddTransaction(transactionIncomeAmount, "desc", balanceId, DateTime.Now, userName, (int)FinansoData.Enum.TransactionTypes.Expense, (int)FinansoData.Enum.TransactionStatuses.Completed, 1);
               
                inMemoryDbTransaction = context.BalanceTransactions.FirstOrDefault();

                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            inMemoryDbTransaction.Should().NotBeNull();

            _balanceManagementRepositoryMock.Verify(x => x.AddToBalanceAmount(balanceId, -transactionIncomeAmount), Times.Once);
        }

        [Fact]
        public async Task AddTransaction_ShouldSubtractFromBalance()
        {
            // Arrange
            RepositoryResult<bool> result;
            BalanceTransaction? inMemoryDbTransaction;
            int balanceId = 1;


            decimal transactionExpensemount = 100m;
            string userName = _appUsers[0].UserName;


            _balanceManagementRepositoryMock.Setup(x => x.AddToBalanceAmount(It.IsAny<int>(), It.IsAny<decimal>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(false));
            _balanceManagementRepositoryMock.Setup(x => x.AddToBalanceAmount(It.Is<int>(id => id == balanceId), It.Is<decimal>(amount => amount == transactionExpensemount)))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));



            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                FinansoData.Repository.Transaction.TransactionManagementRepository repository = new FinansoData.Repository.Transaction.TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);

                // Act
                result = await repository.AddTransaction(transactionExpensemount, "desc", balanceId, DateTime.Now, userName, (int)FinansoData.Enum.TransactionTypes.Income, (int)FinansoData.Enum.TransactionStatuses.Completed, 1);

                inMemoryDbTransaction = context.BalanceTransactions.FirstOrDefault();

                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            inMemoryDbTransaction.Should().NotBeNull();

            _balanceManagementRepositoryMock.Verify(x => x.AddToBalanceAmount(balanceId, transactionExpensemount), Times.Once);
        }

        [Fact]
        public async Task AddTransaction_ShouldNotAddToBalanceWhenTransactionStatusIsNotCompleted()
        {
            // Arrange
            RepositoryResult<bool> result;
            BalanceTransaction? inMemoryDbTransaction;
            int balanceId = 1;


            decimal transactionIncomeAmount = 100m;
            string userName = _appUsers[0].UserName;


            _balanceManagementRepositoryMock.Setup(x => x.AddToBalanceAmount(It.IsAny<int>(), It.IsAny<decimal>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(false));



            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                FinansoData.Repository.Transaction.TransactionManagementRepository repository = new FinansoData.Repository.Transaction.TransactionManagementRepository(context, _balanceManagementRepositoryMock.Object);

                // Act
                result = await repository.AddTransaction(transactionIncomeAmount, "desc", balanceId, DateTime.Now, userName, (int)FinansoData.Enum.TransactionTypes.Expense, (int)FinansoData.Enum.TransactionStatuses.Planned, 1);

                inMemoryDbTransaction = context.BalanceTransactions.FirstOrDefault();

                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            inMemoryDbTransaction.Should().NotBeNull();

            _balanceManagementRepositoryMock.Verify(x => x.AddToBalanceAmount(It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
        }
    }
}
