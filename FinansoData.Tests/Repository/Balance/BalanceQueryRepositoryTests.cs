using FinansoData.Data;
using FinansoData.DataViewModel.Balance;
using FinansoData.Models;
using FinansoData.Repository.Balance;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

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


        public BalanceQueryRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;


            _appUsers = new List<AppUser>
            {
                new AppUser { Id = "1", UserName = "1", FirstName = "1", LastName = "1" },
                new AppUser { Id = "2", UserName = "2", FirstName = "2", LastName = "2" }
            };

            _currencies = new List<FinansoData.Models.Currency>
            {
                new FinansoData.Models.Currency { Id = 1, Name = "PLN" },
                new FinansoData.Models.Currency { Id = 2, Name = "USD" },
                new FinansoData.Models.Currency { Id = 3, Name = "EUR" }
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
        public async Task GetListOfBalancesForUser_WhenUserExists_ReturnsListOfBalances()
        {
            // Arrange
            RepositoryResult<IEnumerable<BalanceViewModel>> result;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context);
                result = await balanceQueryRepository.GetListOfBalancesForUser("1");
            }

            // Act
            //var result = await balanceQueryRepository.GetListOfBalancesForUser(userName);

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
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context);

                result = await balanceQueryRepository.GetListOfBalancesForUser(userName);

            }

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }


        #endregion


        #region GetListOfBalancesForGroup


        [Fact]
        public async Task GetListOfBalancesForGroup_WhenGroupExists_ReturnsListOfBalances()
        {
            // Arrange
            int groupId = 1;
            RepositoryResult<IEnumerable<BalanceViewModel>> result;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IBalanceQueryRepository balanceQueryRepository = new BalanceQueryRepository(context);

                result = await balanceQueryRepository.GetListOfBalancesForGroup(groupId);
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


        #endregion

    }
}
