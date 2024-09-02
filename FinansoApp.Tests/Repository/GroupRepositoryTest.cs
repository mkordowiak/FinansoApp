using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using FinansoData.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinansoApp.Tests.Repository
{
    public class GroupRepositoryTest
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private AppUser _groupOwner;
        private AppUser _groupMember;
        private Group _group;
        private GroupUser _groupUser;


        public GroupRepositoryTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
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
                IEnumerable<GetGroupMembersViewModel> groups = await repository.GetGroupMembersAsync(1);
                // Destroy in-memory database to prevent running multiple instances
                context.Database.EnsureDeleted();

                // Assert
                groups.Should().NotBeNullOrEmpty();
                groups.Count().Should().Be(2);
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
                IEnumerable<GetGroupMembersViewModel> groups = await repository.GetGroupMembersAsync(33);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert
                groups.Count().Should().Be(0);
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
                GetUserMembershipInGroupViewModel memberhip = await repository.GetUserMembershipInGroupAsync(1, "1");
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                memberhip.IsMember.Should().BeFalse();
                memberhip.IsOwner.Should().BeFalse();
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
                GetUserMembershipInGroupViewModel memberhip = await repository.GetUserMembershipInGroupAsync(99, "john@mail.com");
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();


                // Assert 
                memberhip.IsMember.Should().BeFalse();
                memberhip.IsOwner.Should().BeFalse();
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
                GetUserMembershipInGroupViewModel memberhip = await repository.GetUserMembershipInGroupAsync(1, "john@mail.com");
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();


                // Assert 
                memberhip.IsMember.Should().BeTrue();
                memberhip.IsOwner.Should().BeTrue();
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
                GetUserMembershipInGroupViewModel memberhip = await repository.GetUserMembershipInGroupAsync(1, "mark@mail.com");
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();


                // Assert 
                memberhip.IsMember.Should().BeTrue();
                memberhip.IsOwner.Should().BeFalse();
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
                bool data = await groupRepository.Add(newGroupName, groupOwnerUserName);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                data.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupRepository_Add_ShoudBeFalseIfWrongUserId()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupRepository groupRepository = new GroupRepository(context);
                string newGroupName = "New group";
                string groupOwnerUserName = "wrong username";

                // Act
                bool data = await groupRepository.Add(newGroupName, groupOwnerUserName);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                data.Should().BeFalse();
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
                IEnumerable<GetUserGroupsViewModel>? group = await groupRepository.GetUserGroups(userName);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert
                group.Should().NotBeEmpty();
                group.Count().Should().Be(1);
                List<GetUserGroupsViewModel> groupList = group.ToList();
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
                IEnumerable<GetUserGroupsViewModel>? group = await groupRepository.GetUserGroups(userName);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert
                group.Should().NotBeEmpty();
                group.Count().Should().Be(1);
                List<GetUserGroupsViewModel> groupList = group.ToList();
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
                IEnumerable<GetUserGroupsViewModel>? group = await groupRepository.GetUserGroups(userName);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert
                group.Should().NotBeEmpty();
                group.Count().Should().Be(1);
                List<GetUserGroupsViewModel> groupList = group.ToList();
                groupList[0].IsOwner.Should().BeFalse();
            }
        }

        #endregion
    }
}
