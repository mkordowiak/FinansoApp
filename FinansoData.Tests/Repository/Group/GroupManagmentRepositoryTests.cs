using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Tests.Repository.Group
{
    public class GroupManagmentRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private AppUser _group1Owner;
        private AppUser _group1Member;
        private FinansoData.Models.Group _group1;
        private Models.Group _group2;
        private GroupUser _groupUser;

        public GroupManagmentRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                _group1Owner = new AppUser { Id = "1", UserName = "john@mail.com", NormalizedUserName = "JOHN@MAIL.COM", FirstName = "John", LastName = "Doe" };
                _group1Member = new AppUser { Id = "2", UserName = "mark@mail.com", NormalizedUserName = "MARK@MAIL.COM", FirstName = "Mark", LastName = "Knopfler" };

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
        public async Task GroupRepository_Add_ShoudBeTrue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupManagementRepository groupRepository = new GroupManagementRepository(context, groupCrudRepository);

                string newGroupName = "New group";
                string groupOwnerUserName = _group1Owner.NormalizedUserName;

                // Act
                FinansoData.RepositoryResult<bool?> data = await groupRepository.Add(newGroupName, groupOwnerUserName);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                data.Value.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupManagementRepository_Add_ShoudBeFailureIfWrongUserId()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupManagementRepository groupManagementRepository = new GroupManagementRepository(context, groupCrudRepository);

                string newGroupName = "New group";
                string groupOwnerUserName = "wrong username";

                // Act
                FinansoData.RepositoryResult<bool?> data = await groupManagementRepository.Add(newGroupName, groupOwnerUserName);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                data.IsSuccess.Should().BeFalse();
                data.Value.Should().BeNull();
            }
        }

        [Fact]
        public async Task GroupManagementRepository_DeleteGroup_ShouldRemoveGroup()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupManagementRepository groupManagementRepository = new GroupManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.DeleteGroup(_group1.Id);



                // Assert 
                result.IsSuccess.Should().BeTrue();
                context.Groups.Should().HaveCount(1);
                context.Groups.SingleOrDefault().Id.Should().Be(2);

                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();
            }
        }


        [Fact]
        public async Task GroupManagementRepository_DeleteGroupUser_ShouldRetrurnFailureWhenGroupIsIdIsIncorrect()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupManagementRepository groupManagementRepository = new GroupManagementRepository(context, groupCrudRepository);

                // Act
                RepositoryResult<bool> result = await groupManagementRepository.DeleteGroup(999);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert 
                result.IsSuccess.Should().BeFalse();
                result.ErrorType.Should().Be(ErrorType.NotFound);

            }
        }
    }
}
