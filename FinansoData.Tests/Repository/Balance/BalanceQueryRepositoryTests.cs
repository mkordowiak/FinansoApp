using FinansoData.Data;
using FinansoData.DataViewModel.Balance;
using FinansoData.Models;
using FinansoData.Repository;
using FinansoData.Repository.Balance;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoData.Tests.Repository.Balance
{
    public class BalanceQueryRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly List<FinansoData.Models.Balance> _balances;
        private readonly List<FinansoData.Models.AppUser> _appUsers;
        private readonly List<FinansoData.Models.Group> _groups;
        private readonly List<FinansoData.Models.Currency> _currencies;
        private readonly List<FinansoData.Models.GroupUser> _groupUsers;
        private readonly Mock<ICacheWrapper> _cacheWrapper;


        public BalanceQueryRepositoryTests()
        {
            _cacheWrapper = new Mock<ICacheWrapper>();
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;


            _appUsers = new List<AppUser>
            {
                new AppUser { Id = "1", UserName = "1", NormalizedUserName = "1", FirstName = "1", LastName = "1" },
                new AppUser { Id = "2", UserName = "2", NormalizedUserName = "2", FirstName = "2", LastName = "2" }
            };

            _currencies = new List<FinansoData.Models.Currency>
            {
                new FinansoData.Models.Currency { Id = 1, Code = "PLN", Name = "Polski zloty" },
                new FinansoData.Models.Currency { Id = 2, Code = "USD", Name = "Dolar amerykanski" },
                new FinansoData.Models.Currency { Id = 3, Code = "EUR", Name = "Euro" }
            };

            _groups = new List<FinansoData.Models.Group>
            {
                new FinansoData.Models.Group { Id = 1, Name = "Test group 1", OwnerAppUser = _appUsers[0] },
                new FinansoData.Models.Group { Id = 2, Name = "Test group 2", OwnerAppUser = _appUsers[1] },
                new FinansoData.Models.Group { Id = 3, Name = "Test group 3", OwnerAppUser = _appUsers[0] }
            };



            _balances = new List<Models.Balance>
            {
                new Models.Balance { Id = 1, Name = "Bank 1", Amount = 1, Currency = _currencies[0], Group = _groups[0] },
                new Models.Balance { Id = 2, Name = "Bank 2", Amount = 2, Currency = _currencies[1], Group = _groups[2]}
            };

            _groupUsers = new List<GroupUser>
            {
                new GroupUser { Id = 1, Group = _groups[0], AppUser = _appUsers[0], Active = true },
                new GroupUser { Id = 2, Group = _groups[1], AppUser = _appUsers[1], Active = true }
            };


            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Currencies.AddRange(_currencies);
                context.AppUsers.AddRange(_appUsers);
                context.Groups.AddRange(_groups);
                context.Balances.AddRange(_balances);
                context.GroupUsers.AddRange(_groupUsers);

                context.SaveChanges();
            }
        }


        #region GetListOfBalancesForUser


        [Fact]
        public async Task GetListOfBalancesForUser_WhenUserExists_ReturnsListOfBalancesFromDB()
        {
            // Arrange
            RepositoryResult<IEnumerable<BalanceViewModel>> result;

            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<List<BalanceViewModel>>.IsAny))
                .Returns(false);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);

                // Act
                result = await balanceQueryRepository.GetListOfBalancesForUser("1");

                context.Database.EnsureDeleted();
            }

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetListOfBalancesForUser_WhenUserExists_ReturnsListOfBalancesFromCache()
        {
            // Arrange
            RepositoryResult<IEnumerable<BalanceViewModel>> result;

            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<List<BalanceViewModel>>.IsAny))
                .Returns(false);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);

                // Act
                result = await balanceQueryRepository.GetListOfBalancesForUser("1");

                context.Database.EnsureDeleted();
            }


            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
        }


        [Fact]
        public async Task GetListOfBalancesForUser_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            string userName = "nonExistingUser";
            RepositoryResult<IEnumerable<BalanceViewModel>> result;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);

                // Act
                result = await balanceQueryRepository.GetListOfBalancesForUser(userName);

                context.Database.EnsureDeleted();

            }

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }


        #endregion


        #region GetListOfBalancesForGroup


        [Fact]
        public async Task GetListOfBalancesForGroup_WhenGroupExists_ReturnsListOfBalancesFromDB()
        {
            // Arrange
            int groupId = 1;
            RepositoryResult<IEnumerable<BalanceViewModel>> result;

            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<List<BalanceViewModel>>.IsAny))
                .Returns(false);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);

                // Act
                result = await balanceQueryRepository.GetListOfBalancesForGroup(groupId);

                context.Database.EnsureDeleted();
            }

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            Models.Group expectedGroup = _groups.Where(x => x.Id == groupId).First();
            Models.Balance expectedBalance = _balances.Where(x => x.Group.Id == groupId).First();

            result.Value.First().Name.Should().Be(expectedBalance.Name);
            result.Value.First().Amount.Should().Be(expectedBalance.Amount);
            result.Value.First().Currency.Name.Should().Be(expectedBalance.Currency.Name);
            result.Value.First().Group.Name.Should().Be(expectedBalance.Group.Name);
        }

        [Fact]
        public async Task GetListOfBalancesForGroup_WhenGroupExists_ReturnsListOfBalancesFromCache()
        {
            // Arrange
            int groupId = 1;
            RepositoryResult<IEnumerable<BalanceViewModel>> result;


            List<BalanceViewModel> cachedBalances = new List<BalanceViewModel>
            {
                new BalanceViewModel { Id = 999, Name = "Bank 1", Amount = 1, Currency = _currencies[0], Group = _groups[0] }
            };
            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out cachedBalances))
                .Returns(true);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);

                // Act
                result = await balanceQueryRepository.GetListOfBalancesForGroup(groupId);

                context.Database.EnsureDeleted();
            }

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            //repositoryResult.Value[0].Should().BeEquivalentTo(cachedBalances);
            List<BalanceViewModel> resultBalanceList = result.Value.ToList();
            List<BalanceViewModel> cachedBalanceList = cachedBalances.ToList();

            resultBalanceList.Should().BeEquivalentTo(cachedBalanceList);
        }


        [Fact]
        public async Task GetListOfBalancesForGroup_WhenGroupExists_ShoudSaveCache()
        {
            // Arrange
            int groupId = 1;
            RepositoryResult<IEnumerable<BalanceViewModel>> result;

            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<List<BalanceViewModel>>.IsAny))
                .Returns(false);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);

                // Act
                result = await balanceQueryRepository.GetListOfBalancesForGroup(groupId);

                context.Database.EnsureDeleted();
            }

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _cacheWrapper.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<List<BalanceViewModel>>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        #endregion

        #region HasUserAccessToBalance 

        [Fact]
        public async Task HasUserAccessToBalance_WhenUserIsMember_ShoultReturnTrueFromDb()
        {
            // Arrange
            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<bool>.IsAny))
                .Returns(false);

            RepositoryResult<bool?> repositoryResult;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);
                // Act
                repositoryResult = await balanceQueryRepository.HasUserAccessToBalance("1", 1);


                context.Database.EnsureDeleted();
            }
            // Assert
            repositoryResult.Should().NotBeNull();
            repositoryResult.IsSuccess.Should().BeTrue();
            repositoryResult.Value.Should().BeTrue();
        }

        [Fact]
        public async Task HasUserAccessToBalance_WhenUserIsOwner_ShoultReturnTrueFromDb()
        {
            // Arrange
            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<bool>.IsAny))
                .Returns(false);

            RepositoryResult<bool?> repositoryResult;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);
                // Act
                repositoryResult = await balanceQueryRepository.HasUserAccessToBalance("1", 2);


                context.Database.EnsureDeleted();
            }
            // Assert
            repositoryResult.Should().NotBeNull();
            repositoryResult.IsSuccess.Should().BeTrue();
            repositoryResult.Value.Should().BeTrue();
        }

        [Fact]
        public async Task HasUserAccessToBalance_WhenUserIsMember_ShoultReturnTrueFromCache()
        {
            // Arrange
            bool cachedResult = true;
            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out cachedResult))
                .Returns(true);

            RepositoryResult<bool?> repositoryResult;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);
                // Act
                repositoryResult = await balanceQueryRepository.HasUserAccessToBalance("999", 1);

            }

            // Assert
            repositoryResult.Should().NotBeNull();
            repositoryResult.IsSuccess.Should().BeTrue();
            repositoryResult.Value.Should().BeTrue();
        }

        [Fact]
        public async Task HasUserAccessToBalance_WhenUserIsNotMemberOrOwner_ShoultReturnFalse()
        {
            // Arrange
            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<bool>.IsAny))
                .Returns(false);

            RepositoryResult<bool?> repositoryResult;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);
                // Act
                repositoryResult = await balanceQueryRepository.HasUserAccessToBalance("3", 1);


                context.Database.EnsureDeleted();
            }
            // Assert
            repositoryResult.Should().NotBeNull();
            repositoryResult.IsSuccess.Should().BeTrue();
            repositoryResult.Value.Should().BeFalse();
        }



        [Fact]
        public async Task HasUserAccessToBalance_ShoultSaveCache()
        {
            // Arrange
            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<bool>.IsAny))
                .Returns(false);

            RepositoryResult<bool?> repositoryResult;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context, _cacheWrapper.Object);
                // Act
                repositoryResult = await balanceQueryRepository.HasUserAccessToBalance("3", 1);


                context.Database.EnsureDeleted();
            }
            // Assert
            _cacheWrapper.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        #endregion

        #region GetBalance

        [Fact]
        public async Task GetBalance_WhenBalanceExists_ShouldReturnBalanceFromDB()
        {
           // Arrange
           _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<BalanceViewModel>.IsAny))
               .Returns(false);

            RepositoryResult<BalanceViewModel> repositoryResult;
            using (ApplicationDbContext applicationDbContext =  new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(applicationDbContext, _cacheWrapper.Object);
                // Act
                repositoryResult = await balanceQueryRepository.GetBalance(1);
                applicationDbContext.Database.EnsureDeleted();
            }

            // Assert
            repositoryResult.Should().NotBeNull();
            repositoryResult.IsSuccess.Should().BeTrue();
            repositoryResult.Value.Should().NotBeNull();
            repositoryResult.Value.Id.Should().Be(1);
            repositoryResult.Value.Name.Should().Be("Bank 1");
        }

        [Fact]
        public async Task GetBalance_WhenBalanceExists_ShouldReturnBalanceFromCache()
        {
            // Arrange
            BalanceViewModel cachedBalance = new BalanceViewModel
            {
                Id = 1,
                Name = "DATA FROM CACHE",
                Amount = 999,
                Currency = new FinansoData.Models.Currency { Id = 1, Code = "PLN", Name = "Polski zloty" },
                Group = new FinansoData.Models.Group { Id = 1, Name = "Test group 1", OwnerAppUser = new FinansoData.Models.AppUser { Id = "1", UserName = "1", NormalizedUserName = "1", FirstName = "1", LastName = "1" } }
            };
            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out cachedBalance))
                .Returns(true);

            RepositoryResult<BalanceViewModel> repositoryResult;
            using (ApplicationDbContext applicationDbContext = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(applicationDbContext, _cacheWrapper.Object);
                // Act
                repositoryResult = await balanceQueryRepository.GetBalance(1);
                applicationDbContext.Database.EnsureDeleted();
            }

            // Assert
            repositoryResult.Should().NotBeNull();
            repositoryResult.IsSuccess.Should().BeTrue();
            repositoryResult.Value.Should().NotBeNull();
            repositoryResult.Value.Should().BeEquivalentTo(cachedBalance);
        }

        [Fact]
        public async Task GetBalance_WhenBalanceDoesNotExist_ShouldReturnFailure()
        {
            // Arrange
            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<BalanceViewModel>.IsAny))
                .Returns(false);

            RepositoryResult<BalanceViewModel> repositoryResult;
            using (ApplicationDbContext applicationDbContext = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(applicationDbContext, _cacheWrapper.Object);
                // Act
                repositoryResult = await balanceQueryRepository.GetBalance(999);
                applicationDbContext.Database.EnsureDeleted();
            }

            // Assert
            repositoryResult.Should().NotBeNull();
            repositoryResult.IsSuccess.Should().BeFalse();
            repositoryResult.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task GetBalance_ShouldSaveCache()
        {
            // Arrange
            _cacheWrapper.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<BalanceViewModel>.IsAny))
                .Returns(false);

            RepositoryResult<BalanceViewModel> repositoryResult;
            using (ApplicationDbContext applicationDbContext = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(applicationDbContext, _cacheWrapper.Object);
                // Act
                repositoryResult = await balanceQueryRepository.GetBalance(1);
                applicationDbContext.Database.EnsureDeleted();
            }

            // Assert
            _cacheWrapper.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<BalanceViewModel>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        #endregion
    }
}
