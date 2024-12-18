using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using FinansoData.Repository.Balance;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoData.Tests.Repository.Balance
{
    public class BalanceSumAmountTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly List<AppUser> _appUsers;
        private readonly List<Models.Currency> _currencies;
        private readonly List<Models.Group> _groups;
        private readonly List<GroupUser> _groupUsers;
        private readonly List<Models.Balance> _balances;
        private readonly Mock<ICacheWrapper> _cacheWrapperMock;


        public BalanceSumAmountTests()
        {
            _cacheWrapperMock = new Mock<ICacheWrapper>();
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _appUsers = new List<FinansoData.Models.AppUser>
            {
                new Models.AppUser {Id = "1", UserName = "user1", NormalizedUserName = "USER1", FirstName = "User1", LastName = "User1", CreatedAt = DateTime.Now},
                new Models.AppUser {Id = "2", UserName = "user2", NormalizedUserName = "USER2", FirstName = "User2", LastName = "User2", CreatedAt = DateTime.Now},
                new Models.AppUser {Id = "3", UserName = "user3", NormalizedUserName = "USER3", FirstName = "User3", LastName = "User3", CreatedAt = DateTime.Now},
                new Models.AppUser {Id = "4", UserName = "user4", NormalizedUserName = "USER4", FirstName = "User4", LastName = "User4", CreatedAt = DateTime.Now}
            };

            _currencies = new List<Models.Currency>
            {
                new Models.Currency {Id = 1, Name = "USD", Code = "USD", ExchangeRate = 3.88, UpdatedAt = DateTime.Now},
                new Models.Currency {Id = 2, Name = "EUR", Code = "EUR", ExchangeRate = 4.65, UpdatedAt = DateTime.Now},
                new Models.Currency {Id = 3, Name = "PLN", Code = "PLN", ExchangeRate = 1, UpdatedAt = DateTime.Now}
            };

            _groups = new List<Models.Group>
            {
                new Models.Group {Id = 1, Name = "Group1", OwnerAppUser = _appUsers[0], CreatedAt = DateTime.Now},
                new Models.Group {Id = 2, Name = "Group2", OwnerAppUser = _appUsers[3], CreatedAt = DateTime.Now}
            };

            _groupUsers = new List<GroupUser>
            {
                new GroupUser { Id = 1, AppUser= _appUsers[1], Group = _groups[0], CreatedAt = DateTime.Now},
                new GroupUser { Id = 2, AppUser= _appUsers[2], Group = _groups[0], CreatedAt = DateTime.Now},
                new GroupUser { Id = 3, AppUser= _appUsers[2], Group = _groups[1], CreatedAt = DateTime.Now}
            };

            _balances = new List<Models.Balance>
            {
                new Models.Balance {Id = 1, Name = "B0", Amount = 101.66, Currency = _currencies[0], Group = _groups[0], CreatedAt = DateTime.Now},
                new Models.Balance {Id = 2, Name = "B1", Amount = 57.13, Currency = _currencies[1], Group = _groups[0], CreatedAt = DateTime.Now},
                new Models.Balance {Id = 3, Name = "B2", Amount = 1000, Currency = _currencies[2], Group = _groups[0], CreatedAt = DateTime.Now},
                new Models.Balance {Id = 4, Name = "B3", Amount = 15, Currency = _currencies[1], Group = _groups[1], CreatedAt = DateTime.Now},
                new Models.Balance {Id = 5, Name = "B4", Amount = 37000, Currency = _currencies[2], Group = _groups[1], CreatedAt = DateTime.Now},
            };


            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                context.AppUsers.AddRange(_appUsers);
                context.Currencies.AddRange(_currencies);
                context.Groups.AddRange(_groups);
                context.GroupUsers.AddRange(_groupUsers);
                context.Balances.AddRange(_balances);
                context.SaveChanges();
            }
        }

        #region GetGroupBalancesAmount
        [Fact]
        public async Task GetGroupBalancesAmount_ShouldReturnSumOfBalancesAmountForGroupFromDB()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<double?>.IsAny))
                .Returns(false);

            RepositoryResult<double?> repositoryResult1;
            RepositoryResult<double?> repositoryResult2;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                BalanceSumAmount balanceSumAmount = new BalanceSumAmount(context, _cacheWrapperMock.Object);

                // Act
                repositoryResult1 = await balanceSumAmount.GetGroupBalancesAmount(1);
                repositoryResult2 = await balanceSumAmount.GetGroupBalancesAmount(2);
            }

            // Assert 
            repositoryResult1.IsSuccess.Should().BeTrue();
            repositoryResult2.IsSuccess.Should().BeTrue();
            repositoryResult1.Value.Should().NotBeNull();
            repositoryResult2.Value.Should().NotBeNull();
            repositoryResult1.Value.Should().Be(1660.0953);
            repositoryResult2.Value.Should().Be(37069.75);
        }

        [Fact]
        public async Task GetGroupBalancesAmount_ShouldReturnSumOfBalancesAmountForGroupFromCache()
        {
            // Arrange
            double? cacheSum = 999.99;
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out cacheSum))
                .Returns(true);

            RepositoryResult<double?> repositoryResult1;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                BalanceSumAmount balanceSumAmount = new BalanceSumAmount(context, _cacheWrapperMock.Object);

                // Act
                repositoryResult1 = await balanceSumAmount.GetGroupBalancesAmount(1);
            }

            // Assert 
            repositoryResult1.IsSuccess.Should().BeTrue();
            repositoryResult1.Value.Should().NotBeNull();
            repositoryResult1.Value.Should().Be(cacheSum);
        }

        [Fact]
        public async Task GetGroupBalancesAmount_ShouldSaveToCache()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<double?>.IsAny))
                .Returns(false);

            RepositoryResult<double?> repositoryResult1;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                BalanceSumAmount balanceSumAmount = new BalanceSumAmount(context, _cacheWrapperMock.Object);

                // Act
                repositoryResult1 = await balanceSumAmount.GetGroupBalancesAmount(1);
            }

            // Assert 
            repositoryResult1.IsSuccess.Should().BeTrue();
            _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<double?>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        #endregion GetGroupBalancesAmount

        #region GetBalancesSumAmountForUser

        [Fact]
        public async Task GetBalancesSumAmountForUser_ShouldReturnSumOfBalancesAmountForUserFromDB()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<double?>.IsAny))
                .Returns(false);
            RepositoryResult<double?> repositoryResult1;
            RepositoryResult<double?> repositoryResult2;
            RepositoryResult<double?> repositoryResult3;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                BalanceSumAmount balanceSumAmount = new BalanceSumAmount(context, _cacheWrapperMock.Object);
                // Act
                repositoryResult1 = await balanceSumAmount.GetBalancesSumAmountForUser("USER1");
                repositoryResult2 = await balanceSumAmount.GetBalancesSumAmountForUser("USER3");
                repositoryResult3 = await balanceSumAmount.GetBalancesSumAmountForUser("USER4");

            }
            // Assert 
            repositoryResult1.IsSuccess.Should().BeTrue();
            repositoryResult2.IsSuccess.Should().BeTrue();
            repositoryResult3.IsSuccess.Should().BeTrue();

            repositoryResult1.Value.Should().NotBeNull();
            repositoryResult2.Value.Should().NotBeNull();
            repositoryResult3.Value.Should().NotBeNull();

            repositoryResult1.Value.Should().Be(1660.0953);
            repositoryResult2.Value.Should().Be(38729.8453);
            repositoryResult3.Value.Should().Be(37069.75);
        }

        [Fact]
        public async Task GetBalancesSumAmountForUser_ShouldSaveSumToCache()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<double?>.IsAny))
                .Returns(false);
            RepositoryResult<double?> repositoryResult;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                BalanceSumAmount balanceSumAmount = new BalanceSumAmount(context, _cacheWrapperMock.Object);
                // Act
                repositoryResult = await balanceSumAmount.GetBalancesSumAmountForUser("USER1");

            }
            // Assert 
            repositoryResult.IsSuccess.Should().BeTrue();

            _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<double?>(), It.IsAny<TimeSpan>()), Times.Once);

        }

        [Fact]
        public async Task GetBalancesSumAmountForUser_ShouldReturnSumOfBalancesAmountForUserFromCache()
        {
            // Arrange
            double? cacheSum = 999.99;
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out cacheSum))
                .Returns(true);
            RepositoryResult<double?> repositoryResult;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                BalanceSumAmount balanceSumAmount = new BalanceSumAmount(context, _cacheWrapperMock.Object);
                // Act
                repositoryResult = await balanceSumAmount.GetBalancesSumAmountForUser("USER1");

            }
            // Assert 
            repositoryResult.IsSuccess.Should().BeTrue();

            repositoryResult.Value.Should().NotBeNull();

            repositoryResult.Value.Should().Be(cacheSum);
        }

        #endregion

    }
}
