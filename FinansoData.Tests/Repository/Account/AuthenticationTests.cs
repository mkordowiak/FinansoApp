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
        private readonly Mock<ILookupNormalizer> _lookupNormalizerMock;

        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        //private readonly string _username;
        //private readonly string _userEmail;


        private readonly List<AppUser> _appUsers;

        private static readonly object _lock = new object();


        public AuthenticationTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _userManagerMock = new Mock<UserManager<AppUser>>();

            _userStoreMock = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser { UserName = "testuser" });

            _lookupNormalizerMock = new Mock<ILookupNormalizer>();


            // In memory test database
            _appUsers = new List<AppUser>
            {
                new AppUser { Id = Guid.NewGuid().ToString(), NormalizedUserName = "TESTUSER1", UserName = "testUser1", Email = "TEST@USER1.COM", NormalizedEmail = "TEST@USER1.COM" },
                new AppUser { Id = Guid.NewGuid().ToString(), NormalizedUserName = "TESTUSER2", UserName = "testUser2", Email = "TEST@USER2.COM", NormalizedEmail =  "TEST@USER2.COM" },
            };



            //_username = "testUser1";
            //_userEmail = "test@test.com";


            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.ChangeTracker.LazyLoadingEnabled = false;

                string id  = Guid.NewGuid().ToString();

                context.AppUsers.AddRange(_appUsers);


                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnUserWhenUsernameIsCorrect()
        {
            // Arrange

            RepositoryResult<AppUser?> user;
            _lookupNormalizerMock.Setup(x => x.NormalizeName(It.IsAny<string>())).Returns(_appUsers[0].NormalizedUserName);


;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Authentication repository = new Authentication(context, _userManagerMock.Object, _lookupNormalizerMock.Object);

                // Act
                user = await repository.GetUserAsync(_appUsers[0].NormalizedUserName);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();
            }

            // Assert
            user.IsSuccess.Should().BeTrue();
            user.Value.Should().NotBeNull();
            user.Value.Id.Should().Be(_appUsers[0].Id);
        }

        [Fact]
        public async Task GetUserAsync_ShouldReturnNullWhenUsernameIsIncorrect()
        {
            // Arrange
            string incorrectUsername = "123";
            if(_appUsers.Any(x => x.NormalizedUserName.Equals(incorrectUsername)))
            {
                Assert.Fail("This user exist in database, ane it shouldnt");
            }


            RepositoryResult<AppUser?> user;


            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Authentication repository = new Authentication(context, _userManagerMock.Object, _lookupNormalizerMock.Object);


                // Act
                 user = await repository.GetUserAsync(incorrectUsername);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();
            }

            // Assert
            user.IsSuccess.Should().BeTrue();
            user.Value.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShoudResultUserWhenEmailIsCorrect()
        {
            // Arrange 
            RepositoryResult<AppUser?> user;
            _lookupNormalizerMock.Setup(x => x.NormalizeEmail(It.IsAny<string>())).Returns(_appUsers[1].NormalizedEmail);

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Authentication repository = new Authentication(context, _userManagerMock.Object, _lookupNormalizerMock.Object);
                
                // Act
                user = await repository.GetUserByEmailAsync(_appUsers[1].NormalizedEmail);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();
            }

            // Assert
            user.IsSuccess.Should().BeTrue();
            user.Value.Should().NotBeNull();
            user.Value.Id.Should().Be(_appUsers[1].Id);
        }



        [Fact]
        public async Task GetUserByEmailAsync_ShoudResultNullWhenEmailIsIncorrect()
        {
            // Arrange 
            string incorrectEmail = "123";
            if (_appUsers.Any(x => x.NormalizedUserName.Equals(incorrectEmail)))
            {
                Assert.Fail("This user exist in database, ane it shouldnt");
            }

            RepositoryResult<AppUser?> user;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                Authentication repository = new Authentication(context, _userManagerMock.Object, _lookupNormalizerMock.Object);


                // Act
                user = await repository.GetUserByEmailAsync(incorrectEmail);
                // DESTROY IN MEMORY DATABASE - prevent to run multiple instances of database
                context.Database.EnsureDeleted();
            }

            // Assert
            user.IsSuccess.Should().BeTrue();
            user.Value.Should().BeNull();
        }
    }
}
