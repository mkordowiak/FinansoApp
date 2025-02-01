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
    public class GroupQueryRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ICacheWrapper> _cacheWrapperMock;
        private AppUser _group1Owner;
        private AppUser _group1Member;
        private AppUser _group2Member;
        private Models.Group _group1;
        private Models.Group _group2;
        private Models.Group _group3;


        public GroupQueryRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _cacheWrapperMock = new Mock<ICacheWrapper>();

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                _group1Owner = NormalizeAppUserEmail(new AppUser { Id = "1", UserName = "john@mail.com", FirstName = "John", LastName = "Doe" });
                _group1Member = NormalizeAppUserEmail(new AppUser { Id = "2", UserName = "mark@mail.com", FirstName = "Mark", LastName = "Knopfler" });
                _group2Member = NormalizeAppUserEmail(new AppUser { Id = "3", UserName = "andrew@mail.com", FirstName = "Andrew", LastName = "Kmicic" });



                _group1 = new FinansoData.Models.Group { Id = 1, Name = "Group 1", OwnerAppUser = _group1Owner };
                _group2 = new FinansoData.Models.Group { Id = 2, Name = "Group 2", OwnerAppUser = _group1Member };
                _group3 = new FinansoData.Models.Group { Id = 3, Name = "Group 3", OwnerAppUser = _group2Member };

                GroupUser group1UserMember = new GroupUser { Id = 1, Group = _group1, AppUser = _group1Member };
                GroupUser group1UserOwner = new GroupUser { Id = 2, Group = _group1, AppUser = _group1Owner };
                GroupUser group2UserMember = new GroupUser { Id = 3, Group = _group2, AppUser = _group2Member };
                GroupUser group2UserOwner = new GroupUser { Id = 4, Group = _group2, AppUser = _group1Member };
                GroupUser group3UserOwner = new GroupUser { Id = 5, Group = _group3, AppUser = _group2Member };


                context.AppUsers.Add(_group1Owner);
                context.AppUsers.Add(_group1Member);
                context.AppUsers.Add(_group2Member);


                context.Groups.Add(_group1);
                context.Groups.Add(_group2);
                context.Groups.Add(_group3);


                context.GroupUsers.Add(group1UserMember);
                context.GroupUsers.Add(group1UserOwner);
                context.GroupUsers.Add(group2UserMember);
                context.GroupUsers.Add(group2UserOwner);
                context.GroupUsers.Add(group3UserOwner);


                context.SaveChanges();
            }
        }
        private AppUser NormalizeAppUserEmail(AppUser appUser)
        {
            appUser.NormalizedEmail = appUser.UserName;
            return appUser;
        }

        #region IsGroupExists

        [Fact]
        public async Task GroupQueryRepository_IsGroupExists_ShouldReturnTrueIfGroupExists()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<bool> result = await repository.IsGroupExists(_group1.Id);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupQueryRepository_IsGroupExists_ShouldReturnFalseIfGroupDoesNotExists()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context, _cacheWrapperMock.Object);

                // Act 
                RepositoryResult<bool> result = await repository.IsGroupExists(999);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeFalse();
            }
        }

        #endregion

        #region GetUserGroups

        [Fact]
        public async Task GroupQueryRepository_GetUserGroups_ShouldReturnUserGroupsFromDB()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetUserGroupsViewModel>?>.IsAny))
                .Returns(false);

            RepositoryResult<IEnumerable<GetUserGroupsViewModel>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupQueryRepository repository = new GroupQueryRepository(context, _cacheWrapperMock.Object);
                // Act 
                result = await repository.GetUserGroups(_group1Member.NormalizedEmail);
                context.Database.EnsureDeleted();
            }


            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);

            List<GetUserGroupsViewModel> resultGroups = result.Value.ToList();
            resultGroups[0].Id.Should().Be(_group1.Id);
            resultGroups[0].Name.Should().Be(_group1.Name);
            resultGroups[0].IsOwner.Should().BeFalse();
            resultGroups[0].MembersCount.Should().Be(2);

            resultGroups[1].Id.Should().Be(_group2.Id);
            resultGroups[1].Name.Should().Be(_group2.Name);
            resultGroups[1].IsOwner.Should().Be(true);
            resultGroups[1].MembersCount.Should().Be(2);
        }

        [Fact]
        public async Task GroupQueryRepository_GetUserGroups_ShouldSaveCache()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetUserGroupsViewModel>?>.IsAny))
                .Returns(false);

            RepositoryResult<IEnumerable<GetUserGroupsViewModel>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupQueryRepository repository = new GroupQueryRepository(context, _cacheWrapperMock.Object);
                // Act 
                result = await repository.GetUserGroups(_group1Member.NormalizedEmail);
                context.Database.EnsureDeleted();
            }


            // Assert
            result.IsSuccess.Should().BeTrue();
            _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<IEnumerable<GetUserGroupsViewModel>>(), It.IsAny<TimeSpan>()), Times.Once);
        }


        [Fact]
        public async Task GroupQueryRepository_GetUserGroups_ShouldReturnUserGroupsFromCache()
        {
            // Arrange
            IEnumerable<GetUserGroupsViewModel>? cachedGetUserGroupsVM = new List<GetUserGroupsViewModel>
            {
                new GetUserGroupsViewModel { Id = 999, Name = _group1.Name, IsOwner = false, MembersCount = 2 },
                new GetUserGroupsViewModel { Id = 888, Name = _group2.Name, IsOwner = true, MembersCount = 2 },
                new GetUserGroupsViewModel { Id = 777, Name = _group2.Name, IsOwner = true, MembersCount = 2 },
                new GetUserGroupsViewModel { Id = 666, Name = _group2.Name, IsOwner = true, MembersCount = 2 },
            };
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out cachedGetUserGroupsVM))
                .Returns(true);

            RepositoryResult<IEnumerable<GetUserGroupsViewModel>> result;
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupQueryRepository repository = new GroupQueryRepository(context, _cacheWrapperMock.Object);
                // Act 
                result = await repository.GetUserGroups(_group1Member.NormalizedEmail);
                context.Database.EnsureDeleted();
            }


            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(4);

            result.Value.Should().BeEquivalentTo(cachedGetUserGroupsVM);
        }

        #endregion
    }
}
