using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Tests.Repository.Group
{
    public class GroupQueryRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
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
                GroupUser group2UserMember = new GroupUser { Id = 2, Group = _group2, AppUser = _group2Member };


                context.AppUsers.Add(_group1Owner);
                context.AppUsers.Add(_group1Member);
                context.AppUsers.Add(_group2Member);


                context.Groups.Add(_group1);
                context.Groups.Add(_group2);
                context.Groups.Add(_group3);


                context.GroupUsers.Add(group1UserMember);
                context.GroupUsers.Add(group2UserMember);


                context.SaveChanges();
            }
        }



        [Fact]
        public async Task GetGroupMembersAsync_ShouldReturnOwnerAndUser()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context);

                // Act 
                RepositoryResult<IEnumerable<GetGroupMembersViewModel>> result = await repository.GetGroupMembersAsync(_group1.Id);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().HaveCount(2);

                result.Value.Should().Contain(x => x.FirstName == _group1Owner.FirstName && x.LastName == _group1Owner.LastName && x.IsOwner == true);
                result.Value.Should().Contain(x => x.FirstName == _group1Member.FirstName && x.LastName == _group1Member.LastName && x.IsOwner == false);
            }
        }

        [Fact]
        public async Task GetGroupMembersAsync_ShouldRetyrnOnlyOwner()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context);

                // Act 
                RepositoryResult<IEnumerable<GetGroupMembersViewModel>> result = await repository.GetGroupMembersAsync(_group3.Id);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().Contain(x => x.FirstName == _group2Member.FirstName && x.LastName == _group2Member.LastName && x.IsOwner == true);
            }
        }


        private AppUser NormalizeAppUserEmail(AppUser appUser)
        {
            appUser.NormalizedEmail = appUser.UserName;
            return appUser;
        }

        [Fact]
        public async Task GroupRepository_IsUserGroupOwner_ShouldReturnTrueIfUserIsGroupOwner()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context);

                // Act 
                RepositoryResult<bool> result = await repository.IsUserGroupOwner(_group1.Id, _group1Owner.UserName);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeTrue();
            }

        }

        [Fact]
        public async Task GroupRepository_IsUserGroupOwner_ShouldReturnFalseIfUserIsNOTGroupOwner()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context);

                // Act 
                RepositoryResult<bool> result = await repository.IsUserGroupOwner(_group1.Id, _group1Member.UserName);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeFalse();
            }
        }


        [Fact]
        public async Task GroupRepository_IsGroupExists_ShouldReturnTrueIfGroupExists()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context);

                // Act 
                RepositoryResult<bool> result = await repository.IsGroupExists(_group1.Id);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupRepository_IsGroupExists_ShouldReturnFalseIfGroupDoesNotExists()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context);

                // Act 
                RepositoryResult<bool> result = await repository.IsGroupExists(999);
                context.Database.EnsureDeleted();

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeFalse();
            }
        }
    }
}
