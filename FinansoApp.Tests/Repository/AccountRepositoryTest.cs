using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using static FinansoData.Repository.IAccountRepository;
using FluentAssertions;

namespace FinansoApp.Tests.Repository
{
    public class AccountRepositoryTest
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IUserStore<AppUser>> _userStoreMock;

        public AccountRepositoryTest()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _userManagerMock = new Mock<UserManager<AppUser>>();

            _userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser { UserName = "testuser" });
        }
        [Fact]
        public async Task AccountRepository_GetUserAsync_ShouldReturnUserWhenUsernameIsCorrect()
        {
            // Arrange
            string username = "testUser1";
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb") // Keep It Unique To Avoid Collusions
                .Options;

            var accountRepositoryErrorInfoMock = new Mock<IAccountRepositoryErrorInfo>();

            using (ApplicationDbContext context = new ApplicationDbContext(options))
            {
                context.AppUsers.Add(new AppUser { Id = "testId", UserName = username });
                context.SaveChanges();
            }

            // You should repeat the Using block to ensure the operation doesn't share the same DbContext instance.
            // Mocked errors, e.g., mockIErrorInfo, should set up an expectation here as well if required.

            using (ApplicationDbContext context = new ApplicationDbContext(options))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);



                // Act
                var user = await repository.GetUserAsync(username);

                // Assert
                user.Should().NotBeNull();
                user.UserName.Should().Be(username);
            }
        }


        [Fact]
        public async Task AccountRepository_GetUserAsync_ShouldReturnNullWhenUsernameIsIncorrect()
        {
            // Arrange
            string correctUsername = "testUser1";
            string incorrectUsername = "testUser2";

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb") // Keep It Unique To Avoid Collusions
                .Options;

            var accountRepositoryErrorInfoMock = new Mock<IAccountRepositoryErrorInfo>();

            using (ApplicationDbContext context = new ApplicationDbContext(options))
            {
                context.AppUsers.Add(new AppUser { Id = "testId", UserName = correctUsername });
                context.SaveChanges();
            }

            // You should repeat the Using block to ensure the operation doesn't share the same DbContext instance.
            // Mocked errors, e.g., mockIErrorInfo, should set up an expectation here as well if required.

            using (ApplicationDbContext context = new ApplicationDbContext(options))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);



                // Act
                var user = await repository.GetUserAsync(incorrectUsername);

                // Assert
                user.Should().BeNull();
            }
        }

    }
}
