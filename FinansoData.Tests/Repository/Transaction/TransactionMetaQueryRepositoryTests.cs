using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using FinansoData.Repository.Transaction;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;


namespace FinansoData.Tests.Repository.Transaction
{
    public class TransactionMetaQueryRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ApplicationDbContext> _applicationDbContextMock;
        private readonly Mock<ICacheWrapper> _cacheWrapperMock;
        private readonly List<TransactionStatus> _transactionStatuses;
        private readonly List<TransactionType> _transactionTypes;
        private readonly List<TransactionCategory> _transactionCategories;

        public TransactionMetaQueryRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _cacheWrapperMock = new Mock<ICacheWrapper>();

            _transactionStatuses = new List<TransactionStatus>
            {
                new TransactionStatus { Id = 1, Name = "Status1" },
                new TransactionStatus { Id = 2, Name = "Status2" },
                new TransactionStatus { Id = 3, Name = "Status3" }
            };

            _transactionTypes = new List<TransactionType>
            {
                new TransactionType { Id = 1, Name = "Income" },
                new TransactionType { Id = 2, Name = "Expense" }
            };

            _transactionCategories = new List<TransactionCategory>
            {
                new TransactionCategory { Id = 1, Name = "IncomeCategory1", TransactionTypeId = 1, },
                new TransactionCategory { Id = 2, Name = "ExpenseCategory1", TransactionTypeId = 2 }
            };

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.TransactionStatuses.AddRange(_transactionStatuses);
                context.TransactionTypes.AddRange(_transactionTypes);
                context.TransactionCategories.AddRange(_transactionCategories);
                context.SaveChanges();
            }
        }


        #region GetShortListOfAllTransactionStatuses
        [Fact]
        public async Task GetShortListOfAllTransactionStatuses_ShouldReturnDataFromDb()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<List<Tuple<int, string>>>.IsAny))
                .Returns(false);

            RepositoryResult<IEnumerable<Tuple<int, string>>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionMetaQueryRepository transactionMetaQueryRepository = new TransactionMetaQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionMetaQueryRepository.GetShortListOfAllTransactionStatuses();

                // Ensure db is 
                context.Database.EnsureDeleted();
            }
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(result.Value.Count(), _transactionStatuses.Count());
            result.Value.ToList().Should().BeEquivalentTo(_transactionStatuses.Select(x => new Tuple<int, string>(x.Id, x.Name)));
        }

        [Fact]
        public async Task GetShortListOfAllTransactionStatuses_ShouldReturnDataFromCache()
        {
            // Arrange
            IEnumerable<Tuple<int, string>> expected = new List<Tuple<int, string>>
            {
                new Tuple<int, string>(1, "Status1")
            };
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out expected))
                .Returns(true);

            RepositoryResult<IEnumerable<Tuple<int, string>>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionMetaQueryRepository transactionMetaQueryRepository = new TransactionMetaQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionMetaQueryRepository.GetShortListOfAllTransactionStatuses();

                // Ensure db is 
                context.Database.EnsureDeleted();
            }

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(result.Value.Count(), expected.Count());
            result.Value.ToList().Should().BeEquivalentTo(expected);
        }

        #endregion

        #region GetShortListOfAllTransactionTypes
        [Fact]
        public async Task GetShortListOfAllTransactionTypes_ShouldReturnDataFromDb()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<List<Tuple<int, string>>>.IsAny))
                .Returns(false);

            RepositoryResult<IEnumerable<Tuple<int, string>>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionMetaQueryRepository transactionMetaQueryRepository = new TransactionMetaQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionMetaQueryRepository.GetShortListOfAllTransactionTypes();

                // Ensure db is 
                context.Database.EnsureDeleted();
            }
            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(result.Value.Count(), _transactionTypes.Count());
            result.Value.ToList().Should().BeEquivalentTo(_transactionTypes.Select(x => new Tuple<int, string>(x.Id, x.Name)));
        }

        [Fact]
        public async Task GetShortListOfAllTransactionTypes_ShouldReturnDataFromCache()
        {
            // Arrange
            IEnumerable<Tuple<int, string>> expected = new List<Tuple<int, string>>
            {
                new Tuple<int, string>(1, "Type 1")
            };
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out expected))
                .Returns(true);

            RepositoryResult<IEnumerable<Tuple<int, string>>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionMetaQueryRepository transactionMetaQueryRepository = new TransactionMetaQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionMetaQueryRepository.GetShortListOfAllTransactionTypes();

                // Ensure db is 
                context.Database.EnsureDeleted();
            }

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(result.Value.Count(), expected.Count());
            result.Value.ToList().Should().BeEquivalentTo(expected);
        }

        #endregion

        #region GetTransactionIncomeCategories

        [Fact]
        public async Task GetTransactionIncomeCategories_ShouldReturnDataFromDb()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<List<Tuple<int, string>>>.IsAny))
                .Returns(false);

            List<Tuple<int, string >> expected = _transactionCategories
                .Where(x => x.TransactionTypeId.Equals(1))
                .Select(x => new Tuple<int, string>(x.Id, x.Name)).ToList();


            RepositoryResult<IEnumerable<Tuple<int, string>>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionMetaQueryRepository transactionMetaQueryRepository = new TransactionMetaQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionMetaQueryRepository.GetTransactionIncomeCategories();
                // Ensure db is 
                context.Database.EnsureDeleted();
            }
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<List<Tuple<int, string>>>();

            List <Tuple<int, string>> value = (List<Tuple<int, string>>)result.Value;
            value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetTransactionIncomeCategories_ShouldReturnDataFromCache()
        {
            // Arrange
            IEnumerable<Tuple<int, string>> expected = new List<Tuple<int, string>>
            {
                new Tuple<int, string>(1, "CacheIncomeCategory1")
            };
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out expected))
                .Returns(true);
            RepositoryResult<IEnumerable<Tuple<int, string>>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionMetaQueryRepository transactionMetaQueryRepository = new TransactionMetaQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionMetaQueryRepository.GetTransactionIncomeCategories();
                // Ensure db is 
                context.Database.EnsureDeleted();
            }
            // Assert

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<List<Tuple<int, string>>>();

            List<Tuple<int, string>> value = (List<Tuple<int, string>>)result.Value;
            value.Should().BeEquivalentTo(expected);
        }

        #endregion

        #region GetTransactionExpenseCategories

        [Fact]
        public async Task GetTransactionExpenseCategories_ShouldReturnDataFromDb()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<List<Tuple<int, string>>>.IsAny))
                .Returns(false);

            List<Tuple<int, string>> expected = _transactionCategories
                .Where(x => x.TransactionTypeId.Equals(2))
                .Select(x => new Tuple<int, string>(x.Id, x.Name)).ToList();


            RepositoryResult<IEnumerable<Tuple<int, string>>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionMetaQueryRepository transactionMetaQueryRepository = new TransactionMetaQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionMetaQueryRepository.GetTransactionExpenseCategories();
                // Ensure db is 
                context.Database.EnsureDeleted();
            }

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<List<Tuple<int, string>>>();

            List<Tuple<int, string>> value = (List<Tuple<int, string>>)result.Value;
            value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetTransactionExpenseCategories_ShouldReturnDataFromCache()
        {
            // Arrange
            IEnumerable<Tuple<int, string>> expected = new List<Tuple<int, string>>
            {
                new Tuple<int, string>(2, "CachedExpenseCategory1")
            };
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out expected))
                .Returns(true);
            RepositoryResult<IEnumerable<Tuple<int, string>>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                TransactionMetaQueryRepository transactionMetaQueryRepository = new TransactionMetaQueryRepository(context, _cacheWrapperMock.Object);
                // Act
                result = await transactionMetaQueryRepository.GetTransactionExpenseCategories();
                // Ensure db is 
                context.Database.EnsureDeleted();
            }


            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<List<Tuple<int, string>>>();

            List<Tuple<int, string>> value = (List<Tuple<int, string>>)result.Value;
            value.Should().BeEquivalentTo(expected);
        }

        #endregion

    }
}
