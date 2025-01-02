using FinansoData.Data;
using FinansoData.DataViewModel.Balance;
using FinansoData.Repository.Balance;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Tests.Repository.Balance
{
    public class BalanceManagementRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly List<FinansoData.Models.Currency> _currencies;
        private readonly List<FinansoData.Models.Group> _groups;
        private readonly List<FinansoData.Models.Balance> _balances;
        private readonly List<FinansoData.Models.AppUser> _appUsers;

        public BalanceManagementRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                _currencies = new List<Models.Currency>();
                _currencies.Add(new Models.Currency { Id = 1, Code = "PLN", Name = "Polski zloty" });
                _currencies.Add(new Models.Currency { Id = 2, Code = "USD", Name = "Dolar amerykanski" });
                _currencies.Add(new Models.Currency { Id = 3, Code = "EUR", Name = "Euro" });

                _appUsers = new List<Models.AppUser>();
                _appUsers.Add(new Models.AppUser { Id = "1", UserName = "1", FirstName = "1", LastName = "1" });
                _appUsers.Add(new Models.AppUser { Id = "2", UserName = "2", FirstName = "2", LastName = "2" });

                _groups = new List<Models.Group>();
                _groups.Add(new Models.Group { Id = 1, Name = "Test group 1", OwnerAppUser = _appUsers[0] });
                _groups.Add(new Models.Group { Id = 2, Name = "Test group 2", OwnerAppUser = _appUsers[1] });

                _balances = new List<Models.Balance>();
                _balances.Add(new Models.Balance { Id = 1, Name = "Bank", Amount = 1, Currency = _currencies.First(), Group = _groups.First() });

                context.Currencies.AddRange(_currencies);
                context.AppUsers.AddRange(_appUsers);
                context.Groups.AddRange(_groups);
                context.Balances.AddRange(_balances);

                context.SaveChanges();
            }
        }

        #region AddBalance

        [Fact]
        public async Task BalanceManagementRepository_AddBalance_ShouldAddValue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                BalanceManagementRepository balanceManagementRepository = new BalanceManagementRepository(context);


                BalanceViewModel balance = new BalanceViewModel
                {
                    Name = "Test balance",
                    Amount = 3,
                    Currency = _currencies.Last(),
                    Group = _groups.Last()
                };

                // Act
                RepositoryResult<bool> result = await balanceManagementRepository.AddBalance(balance);

                // Assert
                result.IsSuccess.Should().BeTrue();
                context.Balances.Last().Name.Should().Be(balance.Name);
                context.Balances.Last().Amount.Should().Be(balance.Amount);
                context.Balances.Last().CurrencyId.Should().Be(balance.Currency.Id);
                context.Balances.Last().GroupId.Should().Be(balance.Group.Id);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        #endregion

        #region SetBalanceAmount

        [Fact]
        public async Task BalanceManagementRepository_SetBalanceAmount_ShouldUpdateAmount()
        {
            // Arrange

            RepositoryResult<bool?> repositoryResult;
            Models.Balance inMemoryBalance;
            decimal expecedAmount = 5;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                BalanceManagementRepository balanceManagementRepository = new BalanceManagementRepository(context);

                // Act
                repositoryResult = await balanceManagementRepository.SetBalanceAmount(1, expecedAmount);

                inMemoryBalance = context.Balances.First();


                // Delete in-memory database
                context.Database.EnsureDeleted();
            }

            // Assert
            repositoryResult.IsSuccess.Should().BeTrue();
            inMemoryBalance.Amount.Should().Be(expecedAmount);
        }


        [Fact]
        public async Task BalanceManagementRepository_SetBalanceAmount_ShouldReturnFailureWhenCantFindBalance()
        {
            // Arrange

            RepositoryResult<bool?> repositoryResult;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                BalanceManagementRepository balanceManagementRepository = new BalanceManagementRepository(context);

                // Act
                repositoryResult = await balanceManagementRepository.SetBalanceAmount(999, 5);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }

            // Assert
            repositoryResult.IsSuccess.Should().BeFalse();
            repositoryResult.ErrorType.Should().Be(ErrorType.NotFound);
        }

        #endregion
    }
}
