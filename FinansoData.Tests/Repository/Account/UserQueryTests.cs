using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Account;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoData.Tests.Repository.Account
{
    public class UserQueryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ILookupNormalizer> _lookupNormalizerMock;
        private readonly AppUser _user1;

        public UserQueryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _lookupNormalizerMock = new Mock<ILookupNormalizer>();

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                _user1 = new AppUser { Id = "1", UserName = "username", NormalizedUserName = "USERNAME", Email = "john@mail.com", NormalizedEmail = "JOHN@MAIL.COM",  FirstName = "John", LastName = "Doe" };

                context.AppUsers.Add(_user1);
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnUserWhenEmailIsCorrect()
        {
            // Arrange
            RepositoryResult<AppUser?> result;
            _lookupNormalizerMock.Setup(x => x.NormalizeEmail(It.IsAny<string>())).Returns(_user1.NormalizedEmail);
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserQuery userQuery = new UserQuery(context, _lookupNormalizerMock.Object);

                // Act
                result = await userQuery.GetUserByEmail(_user1.Email);

                context.Database.EnsureDeleted();
            }

            // Assert
            Assert.True(result.IsSuccess);
            result.Value.Should().BeEquivalentTo(_user1);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNullWhenEmailINotCorrect()
        {
            // Arrange
            RepositoryResult<AppUser?> result;
            string email = "aaa";
            string normalizedEmail = "AAA";
            _lookupNormalizerMock.Setup(x => x.NormalizeEmail(It.IsAny<string>())).Returns(normalizedEmail);
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserQuery userQuery = new UserQuery(context, _lookupNormalizerMock.Object);

                // Act
                result = await userQuery.GetUserByEmail(email);

                context.Database.EnsureDeleted();
            }

            // Assert
            Assert.True(result.IsSuccess);
            result.Value.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnErrorWhenDbIssue()
        {
            // Arrange
            Mock<ApplicationDbContext> contextMock = new Mock<ApplicationDbContext>(_dbContextOptions);
            UserQuery query = new UserQuery(contextMock.Object, _lookupNormalizerMock.Object);

            // Act
            RepositoryResult<AppUser?> result = await query.GetUserByEmail("email");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal(ErrorType.ServerError, result.ErrorType);
        }

    }
}
