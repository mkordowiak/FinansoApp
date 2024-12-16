using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using FinansoData.Repository;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoData.Tests.Repository.Group
{
    public class GroupUsersQueryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ICacheWrapper> _cacheWrapperMock;
        private AppUser _group1Owner;
        private AppUser _group1Member;
        private AppUser _group2Member;
        private AppUser _group3Invite;
        private Models.Group _group1;
        private Models.Group _group2;
        private Models.Group _group3;


        public GroupUsersQueryTests()
        {
            _cacheWrapperMock = new Mock<ICacheWrapper>();
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                _group1Owner = NormalizeAppUserEmail(new AppUser { Id = "1", UserName = "john@mail.com", FirstName = "John", LastName = "Doe" });
                _group1Member = NormalizeAppUserEmail(new AppUser { Id = "2", UserName = "mark@mail.com", FirstName = "Mark", LastName = "Knopfler" });
                _group2Member = NormalizeAppUserEmail(new AppUser { Id = "3", UserName = "andrew@mail.com", FirstName = "Andrew", LastName = "Kmicic" });
                _group3Invite = NormalizeAppUserEmail(new AppUser { Id = "4", UserName = "eric@mail.com", FirstName = "Eric", LastName = "Clapton" });



                _group1 = new FinansoData.Models.Group { Id = 1, Name = "Group 1", OwnerAppUser = _group1Owner };
                _group2 = new FinansoData.Models.Group { Id = 2, Name = "Group 2", OwnerAppUser = _group1Member };
                _group3 = new FinansoData.Models.Group { Id = 3, Name = "Group 3", OwnerAppUser = _group2Member };

                GroupUser group1UserMember = new GroupUser { Id = 1, Group = _group1, AppUser = _group1Member, Active = true };
                GroupUser group2UserMember = new GroupUser { Id = 2, Group = _group2, AppUser = _group2Member, Active = true };
                GroupUser group3UserMember = new GroupUser { Id = 3, Group = _group1, AppUser = _group3Invite, Active = false };


                context.AppUsers.Add(_group1Owner);
                context.AppUsers.Add(_group1Member);
                context.AppUsers.Add(_group2Member);
                context.AppUsers.Add(_group3Invite);


                context.Groups.Add(_group1);
                context.Groups.Add(_group2);
                context.Groups.Add(_group3);


                context.GroupUsers.Add(group1UserMember);
                context.GroupUsers.Add(group2UserMember);
                context.GroupUsers.Add(group3UserMember);


                context.SaveChanges();
            }
        }

        private AppUser NormalizeAppUserEmail(AppUser appUser)
        {
            appUser.NormalizedEmail = appUser.UserName;
            return appUser;
        }


        [Fact]
        public async Task GetGroupMembersAsync_ShouldReturnOwnerAndUsersWithoutInvitations()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<IEnumerable<GetGroupMembersViewModel>> result = await repository.GetGroupMembersAsync(_group1.Id, false);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().HaveCount(2);

                result.Value.Should().Contain(x => x.FirstName == _group1Owner.FirstName && x.LastName == _group1Owner.LastName && x.IsOwner == true);
                result.Value.Should().Contain(x => x.FirstName == _group1Member.FirstName && x.LastName == _group1Member.LastName && x.IsOwner == false);
            }
        }

        [Fact]
        public async Task GetGroupMembersAsync_ShouldReturnOwnerAndUserWithInvitations()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<IEnumerable<GetGroupMembersViewModel>> result = await repository.GetGroupMembersAsync(_group1.Id, true);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().HaveCount(3);

                result.Value.Should().Contain(x => x.FirstName == _group1Owner.FirstName && x.LastName == _group1Owner.LastName && x.IsOwner == true);
                result.Value.Should().Contain(x => x.FirstName == _group1Member.FirstName && x.LastName == _group1Member.LastName && x.IsOwner == false);
                result.Value.Should().Contain(x => x.FirstName == _group3Invite.FirstName && x.LastName == _group3Invite.LastName && x.IsOwner == false && x.InvitationAccepted == false);
            }
        }



        [Fact]
        public async Task GroupUsersQuery_GetGroupMembersAsync_ShouldReturnOnlyOwner()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<IEnumerable<GetGroupMembersViewModel>> result = await repository.GetGroupMembersAsync(_group3.Id);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().Contain(x => x.FirstName == _group2Member.FirstName && x.LastName == _group2Member.LastName && x.IsOwner == true);
            }
        }

        [Fact]
        public async Task GroupUsersQuery_IsUserGroupOwner_ShouldReturnTrueIfUserIsGroupOwner()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<bool> result = await repository.IsUserGroupOwner(_group1.Id, _group1Owner.UserName);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeTrue();
            }
        }


        [Fact]
        public async Task GroupUsersQuery_IsUserGroupOwner_ShouldReturnFalseIfUserIsNOTGroupOwner()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<bool> result = await repository.IsUserGroupOwner(_group1.Id, _group1Member.UserName);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeFalse();
            }
        }

        [Fact]
        public async Task GroupUsersQuery_GetInvitationCountForGroup_ShouldReturnNum()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<int> result = await repository.GetInvitationCountForGroup(_group3Invite.UserName);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().Be(1);
            }
        }

        [Fact]
        public async Task GroupUsersQuery_GetInvitationCountForGroup_ShouldReturn0IfNoInvitations()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<int> result = await repository.GetInvitationCountForGroup(_group2Member.UserName);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().Be(0);
            }
        }

        [Fact]
        public async Task GroupUsersQuery_IsUserInvited_ShouldReturnTrueIfUserIsInvited()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);

                int groupUserId = context.GroupUsers.Where(x => x.AppUserId == "4").Select(x => x.Id).FirstOrDefault();

                // Act 
                RepositoryResult<bool> result = await repository.IsUserInvited(groupUserId, _group3Invite.UserName);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupUsersQuery_IsUserInvited_ShouldReturnFalseIfUserIsNOTInvited()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<bool> result = await repository.IsUserInvited(100, _group3Invite.UserName);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeFalse();
            }
        }

        #region GetGroupInvitations
        [Fact]
        public async Task GetGroupInvitations_ShouldReturnInvitationsFromDB()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);
                // Act 
                RepositoryResult<IEnumerable<GetGroupInvitationsViewModel>> result = await repository.GetGroupInvitations(_group3Invite.NormalizedEmail);
                context.Database.EnsureDeleted();
                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().HaveCount(1);
            }
        }

        [Fact]
        public async Task GetGroupInvitations_ShouldSaveCache()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);
                // Act 
                RepositoryResult<IEnumerable<GetGroupInvitationsViewModel>> result = await repository.GetGroupInvitations(_group3Invite.NormalizedEmail);
                context.Database.EnsureDeleted();
                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().HaveCount(1);

                _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<IEnumerable<GetGroupInvitationsViewModel>>(), It.IsAny<TimeSpan>()), Times.Once);
            }
        }

        [Fact]
        public async Task GetGroupInvitations_ShouldReturnInvitationsFromCache()
        {
            // Arrange


            IEnumerable<GetGroupInvitationsViewModel> cachedGetGroupInvitationsVM = new List<GetGroupInvitationsViewModel>
            {
                new GetGroupInvitationsViewModel
                {
                    GroupUserId = 888,
                    GroupName = "Cache group name",
                    GroupOwnerFirstName = "Cache owner first name",
                    GroupOwnerLastName = "Cache owner last name",
                    GroupMembersNum = 33
                }
            };


            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out cachedGetGroupInvitationsVM))
                .Returns(true);

            RepositoryResult<IEnumerable<GetGroupInvitationsViewModel>> result;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupUsersQuery repository = new GroupUsersQuery(context, _cacheWrapperMock.Object);
                // Act 
                result = await repository.GetGroupInvitations(_group3Invite.NormalizedEmail);
                context.Database.EnsureDeleted();
            }


            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);

            result.Value.Should().BeEquivalentTo(cachedGetGroupInvitationsVM);
        }

        #endregion
    }
}
