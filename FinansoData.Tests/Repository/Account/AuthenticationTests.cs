using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Account;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace FinansoData.Tests.Repository.Account
{
    [CollectionDefinition("Non-Parallel Collection")]
    public class AuthenticationTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IUserStore<AppUser>> _userStoreMock;

        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly string _username;
        private readonly string _userEmail;

        private static readonly object _lock = new object();


        public AuthenticationTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _userManagerMock = new Mock<UserManager<AppUser>>();

            _userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser { UserName = "testuser" });


            // In memory test database
            _username = "testUser1";
            _userEmail = "test@test.com";


            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.ChangeTracker.LazyLoadingEnabled = false;

                string id  = Guid.NewGuid().ToString();

                context.AppUsers.Add(new AppUser
                {
                    Id = id,
                    UserName = _username,
                    Email = _userEmail,
                    NormalizedEmail = _userEmail,
                    EmailConfirmed = true
                });

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnUserWhenUsernameIsCorrect()
        {
            // Arrange


            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Authentication repository = new Authentication(context, _userManagerMock.Object);


                // Act
                RepositoryResult<AppUser?> user = await repository.GetUserAsync(_username);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.IsSuccess.Should().BeTrue();
                user.Value.Should().NotBeNull();
                user.Value.UserName.Should().Be(_username);
            }
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnNullWhenUsernameIsIncorrect()
        {
            // Arrange
            string incorrectUsername = _username + "123";


            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Authentication repository = new Authentication(context, _userManagerMock.Object);


                // Act
                RepositoryResult<AppUser?> user = await repository.GetUserAsync(incorrectUsername);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.IsSuccess.Should().BeTrue();
                user.Value.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShoudResultUserWhenEmailIsCorrect()
        { 
            // Arrange 
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Authentication repository = new Authentication(context, _userManagerMock.Object);
                
                // Act
                RepositoryResult<AppUser?> user = await repository.GetUserByEmailAsync(_userEmail);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.IsSuccess.Should().BeTrue();
                user.Value.Should().NotBeNull();
                user.Value.Email.Should().Be(_userEmail);
            }
        }



        [Fact]
        public async Task GetUserByEmailAsync_ShoudResultNullWhenEmailIsIncorrect()
        {
            // Arrange 
            string incorrectEmail = _userEmail + "123";

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Authentication repository = new Authentication(context, _userManagerMock.Object);


                // Act
                RepositoryResult<AppUser?> user = await repository.GetUserByEmailAsync(incorrectEmail);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.IsSuccess.Should().BeTrue();
                user.Value.Should().BeNull();
            }
        }
    }
}
