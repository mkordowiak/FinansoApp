using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Account;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;

namespace FinansoData.Tests.Repository.Account
{
    public class UserQueryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly AppUser _user1;

        public UserQueryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                _user1 = new AppUser { Id = "1", UserName = "john@mail.com", FirstName = "John", LastName = "Doe" };

                context.AppUsers.Add(_user1);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnUserWhenEmailIsCorrect()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserQuery userQuery = new UserQuery(context);

                // Act
                RepositoryResult<AppUser?> result = await userQuery.GetUserByEmail(_user1.Email);

                // Assert
                Assert.True(result.IsSuccess);
                result.Value.Should().BeEquivalentTo(_user1);
            }
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNullWhenEmailINotCorrect()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserQuery userQuery = new UserQuery(context);

                // Act
                RepositoryResult<AppUser?> result = await userQuery.GetUserByEmail(_user1.Email + "aaa");

                // Assert
                Assert.True(result.IsSuccess);
                result.Value.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnErrorWhenDbIssue()
        {
            // Arrange
            var contextMock = new Mock<ApplicationDbContext>(_dbContextOptions);
            var query = new UserQuery(contextMock.Object);

            // Act
            var result = await query.GetUserByEmail("email");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal(ErrorType.ServerError, result.ErrorType);
        }

    }
}
