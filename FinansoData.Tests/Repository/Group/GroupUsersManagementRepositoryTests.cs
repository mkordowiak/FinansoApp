using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Tests.Repository.Group
{
    public class GroupUsersManagementRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private AppUser _group1Owner;
        private AppUser _group1Member;
        private FinansoData.Models.Group _group1;
        private Models.Group _group2;
        private GroupUser _groupUser;

        public GroupUsersManagementRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                _group1Owner = new AppUser { Id = "1", UserName = "john@mail.com", FirstName = "John", LastName = "Doe" };
                _group1Member = new AppUser { Id = "2", UserName = "mark@mail.com", FirstName = "Mark", LastName = "Knopfler" };

                _group1 = new FinansoData.Models.Group { Id = 1, Name = "Test _group 1", OwnerAppUser = _group1Owner };
                _group2 = new FinansoData.Models.Group { Id = 2, Name = "Test _group 2", OwnerAppUser = _group1Owner };



                _groupUser = new GroupUser { Id = 1, Group = _group1, AppUser = _group1Member };

                context.AppUsers.Add(_group1Owner);
                context.AppUsers.Add(_group1Member);
                context.Groups.Add(_group1);
                context.Groups.Add(_group2);
                context.GroupUsers.Add(_groupUser);

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GroupManagementRepository_GetUserDeleteInfo_Shoud_ReturnUserDeleteInfo()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository repository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await repository.RemoveUserFromGroup(_groupUser.Id);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                result.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupManagementRepository_DeleteGroupUser_ShouldRemoveGroupUser()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.RemoveUserFromGroup(_groupUser.Id);


                // Assert
                context.GroupUsers.Should().HaveCount(0);


                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task GroupManagementRepository_AddUserToGroup_ShouldReturnnnn()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.AddUserToGroup(_group1, _group1Member);

                // Assert
                context.GroupUsers.Should().HaveCount(2);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task GroupManagementRepository_AddUserToGroup_ShouldAddUserToGroup()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.AddUserToGroup(_group1.Id, _group1Member);

                // Assert
                context.GroupUsers.Should().HaveCount(2);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task GroupManagementRepository_AddUserToGroup_ShouldReturnErrorWhenGroupNotFound()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.AddUserToGroup(100, _group1Member);

                // Assert
                result.IsSuccess.Should().BeFalse();
                result.ErrorType.Should().Be(ErrorType.NotFound);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }
    }
}
