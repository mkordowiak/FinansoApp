using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using FinansoData.Repository.Chart;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Tests.Repository.Chart
{
    public class ChartDataRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ICacheWrapper> _cacheWrapperMock;
        private readonly List<AppUser> _appUsers;
        //private readonly List<TransactionStatus> _transactionStatuses;
        private readonly List<TransactionType> _transactionTypes;
        private readonly List<Models.Group> _groups;
        private readonly List<Models.GroupUser> _groupUsers;
        private readonly List<Models.Balance> _balances;
        private readonly List<BalanceTransaction> _balanceTransactions;
        private readonly List<Models.TransactionCategory> _transactionCategories;

        public ChartDataRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _cacheWrapperMock = new Mock<ICacheWrapper>();

            _appUsers = new List<AppUser>
            {
                new AppUser { Id = "1", UserName = "User1", NormalizedUserName = "NormalizedUser1", NormalizedEmail = "NormalizedEmail1", FirstName = "1", LastName = "1" },
                new AppUser { Id = "2", UserName = "User2", NormalizedUserName = "NormalizedUser2", NormalizedEmail = "NormalizedEmail2", FirstName = "2", LastName = "2" },
                new AppUser { Id = "3", UserName = "User3", NormalizedUserName = "NormalizedUser3", NormalizedEmail = "NormalizedEmail3", FirstName = "3", LastName = "3" }
            };

            _groups = new List<Models.Group>
            {
                new Models.Group { Id = 1, Name = "Group 1", OwnerAppUser = _appUsers[1] },
                new Models.Group { Id = 2, Name = "Group 2", OwnerAppUser = _appUsers[2] }
            };

            _groupUsers = new List<Models.GroupUser>
            {
                new Models.GroupUser { Id = 1, AppUser = _appUsers[0], AppUserId = _appUsers[0].Id, Group = _groups[0], GroupId = _groups[0].Id },
                new Models.GroupUser { Id = 2, AppUser = _appUsers[1], AppUserId = _appUsers[1].Id, Group = _groups[0], GroupId = _groups[0].Id },
                new Models.GroupUser { Id = 3, AppUser = _appUsers[2], AppUserId = _appUsers[2].Id, Group = _groups[1], GroupId = _groups[0].Id },
            };

            _balances = new List<Models.Balance>
            {
                new Models.Balance { Id = 1, Name = "Balance 1", Group = _groups[0], GroupId = _groups[0].Id },
                new Models.Balance { Id = 2, Name = "Balance 2", Group = _groups[0], GroupId = _groups[0].Id },
                new Models.Balance { Id = 3, Name = "Balance 3", Group = _groups[1], GroupId = _groups[1].Id }
            };

            _balanceTransactions = new List<BalanceTransaction>
            {
                new BalanceTransaction { Id = 1, Amount = 100, TransactionDate = DateTime.UtcNow.AddMonths(-1), CreatedAt = DateTime.UtcNow, AppUserId = _appUsers[0].Id, GroupId = _groups[0].Id, BalanceId = _balances[0].Id, TransactionTypeId = 1, TransactionStatusId = 2, TransactionCategoryId = 1, CurrencyId = 1 },
                new BalanceTransaction { Id = 2, Amount = 50, TransactionDate = DateTime.UtcNow.AddMonths(-1), CreatedAt = DateTime.UtcNow, AppUserId = _appUsers[0].Id, GroupId = _groups[0].Id, BalanceId = _balances[0].Id, TransactionTypeId = 1, TransactionStatusId = 2, TransactionCategoryId = 2, CurrencyId = 1 },
                new BalanceTransaction { Id = 3, Amount = 1, TransactionDate = DateTime.UtcNow.AddMonths(-1), CreatedAt = DateTime.UtcNow, AppUserId = _appUsers[0].Id, GroupId = _groups[0].Id, BalanceId = _balances[0].Id, TransactionTypeId = 1, TransactionStatusId = 2, TransactionCategoryId = 2, CurrencyId = 1 },
                new BalanceTransaction { Id = 4, Amount = 5, TransactionDate = DateTime.UtcNow.AddMonths(-1), CreatedAt = DateTime.UtcNow, AppUserId = _appUsers[0].Id, GroupId = _groups[0].Id, BalanceId = _balances[0].Id, TransactionTypeId = 1, TransactionStatusId = 1, TransactionCategoryId = 1, CurrencyId = 1 },
                new BalanceTransaction { Id = 5, Amount = 500, TransactionDate = DateTime.UtcNow.AddMonths(-1), CreatedAt = DateTime.UtcNow, AppUserId = _appUsers[1].Id, GroupId = _groups[0].Id, BalanceId = _balances[0].Id, TransactionTypeId = 2, TransactionStatusId = 2, TransactionCategoryId = 3, CurrencyId = 2 },
                new BalanceTransaction { Id = 6, Amount = 30, TransactionDate = DateTime.UtcNow.AddMonths(-1), CreatedAt = DateTime.UtcNow, AppUserId = _appUsers[1].Id, GroupId = _groups[0].Id, BalanceId = _balances[0].Id, TransactionTypeId = 2, TransactionStatusId = 2, TransactionCategoryId = 4, CurrencyId = 2 },
                new BalanceTransaction { Id = 7, Amount = 2, TransactionDate = DateTime.UtcNow.AddMonths(-1), CreatedAt = DateTime.UtcNow, AppUserId = _appUsers[1].Id, GroupId = _groups[0].Id, BalanceId = _balances[0].Id, TransactionTypeId = 2, TransactionStatusId = 2, TransactionCategoryId = 4, CurrencyId = 2 },
                new BalanceTransaction { Id = 8, Amount = 3, TransactionDate = DateTime.UtcNow.AddMonths(-1), CreatedAt = DateTime.UtcNow, AppUserId = _appUsers[1].Id, GroupId = _groups[0].Id, BalanceId = _balances[0].Id, TransactionTypeId = 2, TransactionStatusId = 1, TransactionCategoryId = 3, CurrencyId = 2 },
                new BalanceTransaction { Id = 9, Amount = 999, TransactionDate = DateTime.UtcNow.AddMonths(-1), CreatedAt = DateTime.UtcNow, AppUserId = _appUsers[2].Id, GroupId = _groups[1].Id, BalanceId = _balances[2].Id, TransactionTypeId = 1, TransactionStatusId = 1, TransactionCategoryId = 1, CurrencyId = 1 }
            };

            _transactionTypes = new List<TransactionType>
            {
                new TransactionType { Id = 1, Name = "Income" },
                new TransactionType { Id = 2, Name = "Expense" }
            };

            _transactionCategories = new List<Models.TransactionCategory>
            {
                new Models.TransactionCategory { Id = 1, Name = "Category 1", TransactionTypeId = 1 },
                new Models.TransactionCategory { Id = 2, Name = "Category 2", TransactionTypeId = 1 },
                new Models.TransactionCategory { Id = 3, Name = "Category 3", TransactionTypeId = 2 },
                new Models.TransactionCategory { Id = 4, Name = "Category 4", TransactionTypeId = 2 }
            };

            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.TransactionTypes.AddRange(_transactionTypes);
                context.TransactionCategories.AddRange(_transactionCategories);
                context.AppUsers.AddRange(_appUsers);
                context.Groups.AddRange(_groups);
                context.GroupUsers.AddRange(_groupUsers);
                context.Balances.AddRange(_balances);
                context.BalanceTransactions.AddRange(_balanceTransactions);
                context.SaveChanges();
            }
        }

        #region GetExpensesInCategories
        [Fact]
        public async Task GetExpensesInCategories_ShouldReturnDataFromDb()
        {
            // Arange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<Tuple<string, decimal>>>.IsAny))
                .Returns(false);


            RepositoryResult<IEnumerable<Tuple<string, decimal>>> result;
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new ChartDataRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await repository.GetExpensesInCategories(_appUsers[0].UserName, 12);

                context.Database.EnsureDeleted();
            }


            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().NotBeEmpty();

            List<Tuple<string, decimal>> resultValue = (List < Tuple<string, decimal> > )result.Value;

            resultValue[0].Item2.Should().Be(500);
            resultValue[1].Item2.Should().Be(32);
        }

        [Fact]
        public async Task GetExpensesInCategories_ShouldReturnDataFromCache()
        {
            // Arange
            IEnumerable<Tuple<string, decimal >>? expected = new List<Tuple<string, decimal>>
            {
                new Tuple<string, decimal>("Category 1", 200),
                new Tuple<string, decimal>("Category 2", 40)
            };


            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out expected))
                .Returns(true);


            RepositoryResult<IEnumerable<Tuple<string, decimal>>> result;
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new ChartDataRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await repository.GetExpensesInCategories(_appUsers[0].UserName, 12);

                context.Database.EnsureDeleted();
            }


            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().NotBeEmpty();

            List<Tuple<string, decimal>> resultValue = (List<Tuple<string, decimal>>)result.Value;

            resultValue[0].Item2.Should().Be(200);
            resultValue[1].Item2.Should().Be(40);
        }

        #endregion

        #region GetIncomesInCategories

        [Fact]
        public async Task GetIncomesInCategories_ShouldReturnDataFromDb()
        {
            // Arange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<Tuple<string, decimal>>>.IsAny))
                .Returns(false);


            RepositoryResult<IEnumerable<Tuple<string, decimal>>> result;
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new ChartDataRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await repository.GetIncomesInCategories(_appUsers[0].UserName, 12);

                context.Database.EnsureDeleted();
            }


            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().NotBeEmpty();

            List<Tuple<string, decimal>> resultValue = (List<Tuple<string, decimal>>)result.Value;

            resultValue[0].Item2.Should().Be(100);
            resultValue[1].Item2.Should().Be(51);
        }

        [Fact]
        public async Task GetIncomesInCategories_ShouldReturnDataFromCache()
        {
            // Arange
            IEnumerable<Tuple<string, decimal>>? expected = new List<Tuple<string, decimal>>
            {
                new Tuple<string, decimal>("Category 1", 700),
                new Tuple<string, decimal>("Category 2", 80)
            };


            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out expected))
                .Returns(true);


            RepositoryResult<IEnumerable<Tuple<string, decimal>>> result;
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new ChartDataRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await repository.GetIncomesInCategories(_appUsers[0].UserName, 12);

                context.Database.EnsureDeleted();
            }


            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().NotBeEmpty();

            List<Tuple<string, decimal>> resultValue = (List<Tuple<string, decimal>>)result.Value;

            resultValue[0].Item2.Should().Be(700);
            resultValue[1].Item2.Should().Be(80);
        }

        #endregion
    }
}
