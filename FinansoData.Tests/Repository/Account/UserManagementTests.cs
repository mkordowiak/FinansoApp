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

namespace FinansoData.Tests.Repository.Account
{
    public class UserManagementTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IUserStore<AppUser>> _userStoreMock;

        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly string _username;
        private readonly string _userEmail;

        private AppUser _user1;

        public UserManagementTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _userManagerMock = new Mock<UserManager<AppUser>>();

            _userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser { UserName = "testuser" });


            // In memory test database
            _user1 = new AppUser
            {
                UserName = "testUser1",
                Email = "test@test.com",
                EmailConfirmed = true,
                Id = "1",
                FirstName = "Konrad",
                LastName = "Walenrod"
            };


            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.AppUsers.Add(_user1);


                context.SaveChanges();
            }
        }


        [Fact]
        public async Task CreateAppUser_ShouldResultErrorWhenUserExists()
        {
            // Arrange
            string password = "password123!";
            Mock<IAuthentication> authenticationMock = new Mock<IAuthentication>();

            RepositoryResult<AppUser?> getUserByEmailAsync = RepositoryResult<AppUser?>
                .Success(
                    new AppUser { UserName = "testuser" }
                );

            authenticationMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
               .ReturnsAsync(getUserByEmailAsync);



            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                UserManagement repository = new UserManagement(context, _userManagerMock.Object, authenticationMock.Object);

                // Act
                RepositoryResult<AppUser?> user = await repository.CreateAppUser(_userEmail, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.IsSuccess.Should().BeFalse();
                user.ErrorType.Should().Be(ErrorType.EmailAlreadyExists);
            }
        }

        [Fact]
        public async Task UserManagement_CreateAppUser_ShouldResultErrorWhenUserExists()
        {
            // Arrange
            string password = "password123!";
            Mock<IAuthentication> authenticationMock = new Mock<IAuthentication>();

            RepositoryResult<AppUser?> getUserByEmailAsync = RepositoryResult<AppUser?>
                .Success(
                    new AppUser { UserName = "testuser" }
                );

            authenticationMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(getUserByEmailAsync);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserManagement repository = new UserManagement(context, _userManagerMock.Object, authenticationMock.Object);

                // Act
                RepositoryResult<AppUser?> user = await repository.CreateAppUser(_userEmail, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.IsSuccess.Should().BeFalse();
                user.Value.Should().BeNull();
                user.ErrorType.Should().Be(ErrorType.EmailAlreadyExists);
            }
        }

        [Fact]
        public async Task UserManagement_CreateAppUser_ShouldCreateUser()
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

            Mock<IAuthentication> authenticationMock = new Mock<IAuthentication>();

            RepositoryResult<AppUser?> getUserByEmailAsync = RepositoryResult<AppUser?>.Success(null);

            authenticationMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(getUserByEmailAsync);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserManagement repository = new UserManagement(context, _userManagerMock.Object, authenticationMock.Object);


                // Act
                RepositoryResult<AppUser?> user = await repository.CreateAppUser(email, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.IsSuccess.Should().BeTrue();
                user.Value.Should().NotBeNull();
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

            Mock<IAuthentication> authenticationMock = new Mock<IAuthentication>();
            RepositoryResult<AppUser?> getUserByEmailAsync = RepositoryResult<AppUser?>.Success(null);
            authenticationMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(getUserByEmailAsync);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserManagement repository = new UserManagement(context, _userManagerMock.Object, authenticationMock.Object);


                // Act
                RepositoryResult<AppUser?> user = await repository.CreateAppUser(email, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                user.IsSuccess.Should().BeFalse();
                user.ErrorType.Should().Be(ErrorType.AssignUserRoleError);
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

            Mock<IAuthentication> authenticationMock = new Mock<IAuthentication>();

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserManagement repository = new UserManagement(context, _userManagerMock.Object, authenticationMock.Object);

                // Act
                RepositoryResult<bool> result = await repository.AdminSetNewPassword(appUser, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeFalse();
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

            Mock<IAuthentication> authenticationMock = new Mock<IAuthentication>();

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserManagement repository = new UserManagement(context, _userManagerMock.Object, authenticationMock.Object);


                // Act
                RepositoryResult<bool> result = await repository.AdminSetNewPassword(appUser, password);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess!.Should().BeTrue();
                result.ErrorType.Should().Be(ErrorType.None);
            }
        }

        #region EditUserInfo

        [Fact]
        public async Task UserManagement_EditUserInfo_ShouldChangeEntry()
        {
            // Arrange
            AppUser updateAppUser = (AppUser)_user1;
            string NewFirstName = "NewFirstName";
            string NewLastName = "NewLastName";

            updateAppUser.FirstName = NewFirstName;
            updateAppUser.LastName = NewLastName;

            Mock<IAuthentication> authenticationMock = new Mock<IAuthentication>();

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                UserManagement repository = new UserManagement(context, _userManagerMock.Object, authenticationMock.Object);

                // Act
                RepositoryResult<bool> result = await repository.EditUserInfo(updateAppUser);
                

                // Assert
                result.IsSuccess.Should().BeTrue();
                context.AppUsers.FirstOrDefault(x => x.Id == updateAppUser.Id).Should().NotBeNull();
                context.AppUsers.FirstOrDefault(x => x.Id == updateAppUser.Id).FirstName.Should().Be(NewFirstName);
                context.AppUsers.FirstOrDefault(x => x.Id == updateAppUser.Id).LastName.Should().Be(NewLastName);

                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();

            }
        }

        #endregion
    }
}
