using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using FinansoData.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Tests.Repository
{
    public class GroupRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private AppUser _groupOwner;
        private AppUser _groupMember;
        private Group _group;
        private GroupUser _groupUser;


        public GroupRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                _groupOwner = new AppUser { Id = "1", UserName = "john@mail.com", FirstName = "John", LastName = "Doe" };
                _groupMember = new AppUser { Id = "2", UserName = "mark@mail.com", FirstName = "Mark", LastName = "Knopfler" };

                _group = new Group { Id = 1, Name = "Test _group 1", OwnerAppUser = _groupOwner };

                _groupUser = new GroupUser { Id = 1, Group = _group, AppUser = _groupMember };

                context.AppUsers.Add(_groupOwner);
                context.AppUsers.Add(_groupMember);
                context.Groups.Add(_group);
                context.GroupUsers.Add(_groupUser);

                context.SaveChanges();
            }
        }


        #region GetGroupMembersAsync
        [Fact]
        public async Task GroupRepository_GetGroupMembersAsync_ShouldNotBeNull()
        {
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                // Arrange
                GroupRepository repository = new GroupRepository(context);

                // Act 
                FinansoData.RepositoryResult<IEnumerable<GetGroupMembersViewModel>> groups = await repository.GetGroupMembersAsync(1);
                // Destroy in-memory database to prevent running multiple instances
                context.Database.EnsureDeleted();

                // Assert
                groups.Value.Should().NotBeNullOrEmpty();
                groups.Value.Count().Should().Be(2);
            }
        }

        [Fact]
        public async Task GroupRepository_GetGroupMembersAsync_ShouldBeNullIfIdIsEmpty()
        {
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                // Arrange
                GroupRepository repository = new GroupRepository(context);

                // Act 
                FinansoData.RepositoryResult<IEnumerable<GetGroupMembersViewModel>> groups = await repository.GetGroupMembersAsync(33);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert
                groups.Value.Count().Should().Be(0);
            }
        }

        #endregion



        #region GetUserMembershipInGroup
        [Fact]
        public async Task GroupRepository_GetUserMembershipInGroup_ShoudReturnFalseWhenUserIdIsWrong()
        {
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                // Arrange
                GroupRepository repository = new GroupRepository(context);

                // Act 
                FinansoData.RepositoryResult<GetUserMembershipInGroupViewModel> memberhip = await repository.GetUserMembershipInGroupAsync(1, "1");
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                memberhip.Value.IsMember.Should().BeFalse();
                memberhip.Value.IsOwner.Should().BeFalse();
            }
        }

        [Fact]
        public async Task GroupRepository_GetUserMembershipInGroup_ShoudReturnFalseWhenGroupIdIsWrong()
        {
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                // Arrange
                GroupRepository repository = new GroupRepository(context);

                // Act 
                FinansoData.RepositoryResult<GetUserMembershipInGroupViewModel> memberhip = await repository.GetUserMembershipInGroupAsync(99, "john@mail.com");
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();


                // Assert 
                memberhip.Value.IsMember.Should().BeFalse();
                memberhip.Value.IsOwner.Should().BeFalse();
            }
        }

        [Fact]
        public async Task GroupRepository_GetUserMembershipInGroup_ShoudReturnTrueWhenOwnerIsGiven()
        {
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                // Arrange
                GroupRepository repository = new GroupRepository(context);

                // Act 
                FinansoData.RepositoryResult<GetUserMembershipInGroupViewModel> memberhip = await repository.GetUserMembershipInGroupAsync(1, "john@mail.com");
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();


                // Assert 
                memberhip.Value.IsMember.Should().BeTrue();
                memberhip.Value.IsOwner.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupRepository_GetUserMembershipInGroup_ShoudReturnTrueWhenMemberIsGiven()
        {
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                // Arrange
                GroupRepository repository = new GroupRepository(context);

                // Act 
                FinansoData.RepositoryResult<GetUserMembershipInGroupViewModel> memberhip = await repository.GetUserMembershipInGroupAsync(1, "mark@mail.com");
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();


                // Assert 
                memberhip.Value.IsMember.Should().BeTrue();
                memberhip.Value.IsOwner.Should().BeFalse();
            }
        }

        #endregion

        #region Add
        [Fact]
        public async Task GroupRepository_Add_ShoudBeTrue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupRepository groupRepository = new GroupRepository(context);
                string newGroupName = "New group";
                string groupOwnerUserName = _groupOwner.UserName;

                // Act
                FinansoData.RepositoryResult<bool?> data = await groupRepository.Add(newGroupName, groupOwnerUserName);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                data.Value.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupRepository_Add_ShoudBeFailureIfWrongUserId()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupRepository groupRepository = new GroupRepository(context);
                string newGroupName = "New group";
                string groupOwnerUserName = "wrong username";

                // Act
                FinansoData.RepositoryResult<bool?> data = await groupRepository.Add(newGroupName, groupOwnerUserName);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                data.IsSuccess.Should().BeFalse();
                data.Value.Should().BeNull();
            }
        }

        #endregion

        #region GetUserGroups

        [Fact]
        public async Task GroupRepository_GetUserGroups_ReurnGroupWhenMemberPassed()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupRepository groupRepository = new GroupRepository(context);
                string userName = _groupMember.UserName;


                // Act
                FinansoData.RepositoryResult<IEnumerable<GetUserGroupsViewModel>?> group = await groupRepository.GetUserGroups(userName);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert
                group.Value.Should().NotBeEmpty();
                group.Value.Count().Should().Be(1);
                List<GetUserGroupsViewModel> groupList = group.Value.ToList();
                groupList[0].IsOwner.Should().BeFalse();
            }
        }

        [Fact]
        public async Task GroupRepository_GetUserGroups_ReurnGroupWhenOwnerPassed()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupRepository groupRepository = new GroupRepository(context);
                string userName = _groupOwner.UserName;


                // Act
                FinansoData.RepositoryResult<IEnumerable<GetUserGroupsViewModel>?> group = await groupRepository.GetUserGroups(userName);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert
                group.Value.Should().NotBeEmpty();
                group.Value.Count().Should().Be(1);
                List<GetUserGroupsViewModel> groupList = group.Value.ToList();
                groupList[0].IsOwner.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupRepository_GetUserGroups_ReurnGroupWhenWrongIdIsPassed()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupRepository groupRepository = new GroupRepository(context);
                string userName = _groupMember.UserName;


                // Act
                FinansoData.RepositoryResult<IEnumerable<GetUserGroupsViewModel>?> group = await groupRepository.GetUserGroups(userName);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert
                group.Value.Should().NotBeEmpty();
                group.Value.Count().Should().Be(1);
                List<GetUserGroupsViewModel> groupList = group.Value.ToList();
                groupList[0].IsOwner.Should().BeFalse();
            }
        }

        #endregion
    }
}
