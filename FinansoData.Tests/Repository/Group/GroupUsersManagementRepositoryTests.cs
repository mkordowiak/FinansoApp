using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Tests.Repository.Group
{
    public class GroupUsersManagementRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private AppUser _group1Owner;
        private AppUser _group1Member;
        private AppUser _group1InvitationInactive;
        private FinansoData.Models.Group _group1;
        private Models.Group _group2;
        private GroupUser _groupUser;
        private GroupUser _groupInvitation;

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
                _group1InvitationInactive = new AppUser { Id = "3", UserName = "eric@mail.com", FirstName = "Eric", LastName = "Clapton" };

                _group1 = new FinansoData.Models.Group { Id = 1, Name = "Test _group 1", OwnerAppUser = _group1Owner };
                _group2 = new FinansoData.Models.Group { Id = 2, Name = "Test _group 2", OwnerAppUser = _group1Owner };



                _groupUser = new GroupUser { Id = 1, Group = _group1, AppUser = _group1Member };
                _groupInvitation = new GroupUser { Id = 2, Group = _group1, AppUser = _group1InvitationInactive, Active = false };


                context.AppUsers.Add(_group1Owner);
                context.AppUsers.Add(_group1Member);
                context.Groups.Add(_group1);
                context.Groups.Add(_group2);
                context.GroupUsers.Add(_groupUser);
                context.GroupUsers.Add(_groupInvitation);

                context.SaveChanges();
            }
        }


        #region RemoveUserFromGroup
        [Fact]
        public async Task GroupManagementRepository_RemoveUserFromGroup_Shoud_ReturnUserDeleteInfo()
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
        public async Task GroupUsersManagementRepository_RemoveUserFromGroup_ShouldRemoveGroupUser()
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

        #endregion

        #region AddUserToGroup

        [Fact]
        public async Task GroupUsersManagementRepository_AddUserToGroup_ShouldAddItem()
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
        public async Task GroupUsersManagementRepository_AddUserToGroup_NewItemShouldNotBeActive()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.AddUserToGroup(_group1, _group1Member);

                // Assert
                context.GroupUsers.Last().Active.Should().BeFalse();

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task GroupUsersManagementRepository_AddUserToGroup_ShouldAddUserToGroup()
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
        public async Task GroupUsersManagementRepository_AddUserToGroup_ShouldReturnErrorWhenGroupNotFound()
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

        #endregion

        #region RejectGroupInvitation

        [Fact]
        public async Task GroupUsersManagementRepository_RejestGroupInvitation_ShouldReturnErrorWhenGroupUserNotFound()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.RejectGroupInvitation(100);

                // Assert
                result.IsSuccess.Should().BeFalse();
                result.ErrorType.Should().Be(ErrorType.NotFound);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }


        [Fact]
        public async Task GroupUsersManagementRepository_RejectGroupInvitation_ShouldDeleteGroupUserEntry()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.RejectGroupInvitation(_groupInvitation.Id);

                // Assert
                result.IsSuccess.Should().BeTrue();
                context.GroupUsers.Where(x => x.Id == _groupInvitation.Id).FirstOrDefault().Should().BeNull();

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }

        #endregion

        #region AcceptGroupInvitation

        [Fact]
        public async Task GroupUsersManagementRepository_AcceptGroupInvitation_ShouldReturnErrorWhenGroupUserNotFound()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.AcceptGroupInvitation(100);

                // Assert
                result.IsSuccess.Should().BeFalse();
                result.ErrorType.Should().Be(ErrorType.NotFound);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task GroupUsersManagementRepository_AcceptGroupInvitation_ShouldActivateGroupUserEntry()
        {
            // Arrange 
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.AcceptGroupInvitation(_groupInvitation.Id);

                // Assert
                result.IsSuccess.Should().BeTrue();
                context.GroupUsers.Where(x => x.Id == _groupInvitation.Id).First().Active.Should().BeTrue();

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task GroupUsersManagementRepository_AcceptGroupInvitation_ShouldRetrunNotFoundWhenGroupUserAlreadyActive()
        {
            // Arrange 
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupUsersManagementRepository groupManagementRepository = new GroupUsersManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.AcceptGroupInvitation(_groupUser.Id);

                // Assert
                result.IsSuccess.Should().BeFalse();
                result.ErrorType.Should().Be(ErrorType.NotFound);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }

        #endregion
    }
}
