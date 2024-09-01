using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoApp.Tests.Repository
{
    public class AccountRepositoryTest
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IUserStore<AppUser>> _userStoreMock;


        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly string _username;
        private readonly string _userEmail;

        public AccountRepositoryTest()
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
                .UseInMemoryDatabase(databaseName: "TestDb") 
            .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                context.AppUsers.Add(new AppUser
                {
                    Id = "testId",
                    UserName = _username,
                    Email = _userEmail,
                    EmailConfirmed = true
                });


                context.SaveChanges();
            }
        }


        [Fact]
        public async Task AccountRepository_GetUserAsync_ShouldReturnUserWhenUsernameIsCorrect()
        {
            // Arrange


            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);



                // Act
                AppUser? user = await repository.GetUserAsync(_username);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.Should().NotBeNull();
                user.UserName.Should().Be(_username);
            }
        }


        [Fact]
        public async Task AccountRepository_GetUserAsync_ShouldReturnNullWhenUsernameIsIncorrect()
        {
            // Arrange
            string incorrectUsername = _username + "123";



            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);


                // Act
                AppUser? user = await repository.GetUserAsync(incorrectUsername);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.Should().BeNull();
            }
        }

        [Fact]
        public async Task AccountRepository_GetUserByEmailAsync_ShoudResultUserWhenEmailIsCorrect()
        {
            // Arrange 

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);


                // Act
                AppUser? user = await repository.GetUserByEmailAsync(_userEmail);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.Should().NotBeNull();
                user.Email.Should().Be(_userEmail);
            }
        }



        [Fact]
        public async Task AccountRepository_GetUserByEmailAsync_ShoudResultNullWhenEmailIsIncorrect()
        {
            // Arrange 
            string incorrectEmail = _userEmail + "123";

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);


                // Act
                AppUser? user = await repository.GetUserByEmailAsync(incorrectEmail);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.Should().BeNull();
            }
        }

        [Fact]
        public async Task AccountRepository_CreateAppUser_ShouldResultErrorWhenUserExists()
        {
            // Arrange
            string password = "password123!";

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);



                // Act
                AppUser? user = await repository.CreateAppUser(_userEmail, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.Should().BeNull();
                repository.Error.EmailAlreadyExists.Should().BeTrue();
            }
        }

        [Fact]
        public async Task AccountRepository_CreateAppUser_ShouldCreateUser()
        {
            // Arrange
            string password = "pasSword123!";
            string email = _userEmail + "123";

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);


                // Act
                AppUser? user = await repository.CreateAppUser(email, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.Should().NotBeNull();
                repository.Error.EmailAlreadyExists.Should().BeFalse();
                repository.Error.RegisterError.Should().BeFalse();
            }
        }

        [Fact]
        public async Task AccountRepository_CreateAppUser_ShouldResultErrorWhenCantAssignRole()
        {
            // Arrange
            string password = "pasSword123!";
            string email = _userEmail + "123";

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);


                // Act
                AppUser? user = await repository.CreateAppUser(email, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.Should().BeNull();
                repository.Error.EmailAlreadyExists.Should().BeFalse();
                repository.Error.AssignUserRoleError.Should().BeTrue();
            }
        }

        [Fact]
        public async Task AccountRepository_AdminSetNewPassword_ShoudResultErrorWhenCantPerform()
        {
            // Arrange
            string password = "pasSword123!";
            AppUser appUser = new AppUser
            {
                Email = _userEmail,
                UserName = _userEmail
            };

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);


                // Act
                bool? result = await repository.AdminSetNewPassword(appUser, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                result.Should().BeNull();
                repository.Error.DatabaseError.Should().BeTrue();
            }
        }


        [Fact]
        public async Task AccountRepository_AdminSetNewPassword_ShoudReturnTrueWhenOk()
        {
            // Arrange
            string password = "pasSword123!";
            AppUser appUser = new AppUser
            {
                Email = _userEmail,
                UserName = _userEmail
            };

            _userManagerMock
                .Setup(x => x.RemovePasswordAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(x => x.AddPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                AccountRepository repository = new AccountRepository(context, _userManagerMock.Object);


                // Act
                bool? result = await repository.AdminSetNewPassword(appUser, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                result.Should().NotBeNull();
                result.Should().BeTrue();
                repository.Error.DatabaseError.Should().BeFalse();
            }
        }
    }
}
