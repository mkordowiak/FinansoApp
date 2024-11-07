using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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

                GroupUser group1UserMember = new GroupUser { Id = 1, Group = _group1, AppUser = _group1Member };
                GroupUser group2UserMember = new GroupUser { Id = 2, Group = _group2, AppUser = _group2Member };


                context.AppUsers.Add(_group1Owner);
                context.AppUsers.Add(_group1Member);
                context.AppUsers.Add(_group2Member);


                context.Groups.Add(_group1);
                context.Groups.Add(_group2);


                context.GroupUsers.Add(group1UserMember);
                context.GroupUsers.Add(group2UserMember);

                context.SaveChanges();
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
                var result = await repository.IsUserGroupOwner(_group1.Id, _group1Owner.UserName);

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
                var result = await repository.IsUserGroupOwner(_group1.Id, _group1Member.UserName);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeFalse();
            }
        }


        [Fact]
        public async Task GroupRepository_IsGroupExistsShouldReturnTrueIfGroupExists()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context);

                // Act 
                var result = await repository.IsGroupExists(_group1.Id);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeTrue();
            }
        }

        [Fact]
        public async Task GroupRepository_IsGroupExistsShouldReturnFalseIfGroupDoesNotExists()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {

                GroupQueryRepository repository = new GroupQueryRepository(context);

                // Act 
                var result = await repository.IsGroupExists(999);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeFalse();
            }
        }
    }
}
