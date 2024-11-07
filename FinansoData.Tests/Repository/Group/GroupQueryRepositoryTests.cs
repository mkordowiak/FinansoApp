using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Tests.Repository.Group
{
    public class GroupQueryRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private AppUser _groupOwner;
        private AppUser _groupMember;
        private FinansoData.Models.Group _group;
        private GroupUser _groupUser;

        public GroupQueryRepositoryTests()
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

                _group = new FinansoData.Models.Group { Id = 1, Name = "Test _group 1", OwnerAppUser = _groupOwner };

                _groupUser = new GroupUser { Id = 1, Group = _group, AppUser = _groupMember };

                context.AppUsers.Add(_groupOwner);
                context.AppUsers.Add(_groupMember);
                context.Groups.Add(_group);
                context.GroupUsers.Add(_groupUser);

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GetGroupMembersAsync_ShouldReturnValues()
        {
            // Arrange 
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupQueryRepository groupQueryRepository = new GroupQueryRepository(context);

                RepositoryResult<IEnumerable<DataViewModel.Group.GetGroupMembersViewModel>> result = await groupQueryRepository.GetGroupMembersAsync(1);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().NotBeNull();
            }
        }


        [Fact]
        public async Task GetGroupMembersAsync_ShoudReturnNullWhenIdIsWrong()
        {
            // Arrange 
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupQueryRepository groupQueryRepository = new GroupQueryRepository(context);

                RepositoryResult<IEnumerable<DataViewModel.Group.GetGroupMembersViewModel>> result = await groupQueryRepository.GetGroupMembersAsync(10);

                // Assert
                result.IsSuccess.Should().BeTrue();

                IEnumerable<DataViewModel.Group.GetGroupMembersViewModel> value = result.Value;
                value.Should().NotBeNull();
                value.Should().BeEmpty();
            }
        }


    }
}
