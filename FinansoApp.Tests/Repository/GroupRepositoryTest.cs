using FinansoData.Data;
using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using FinansoData.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinansoApp.Tests.Repository
{
    public class GroupRepositoryTest
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public GroupRepositoryTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                AppUser groupOwner = new AppUser { Id = "1", UserName = "john@mail.com", FirstName = "John", LastName = "Doe" };
                AppUser groupMember = new AppUser { Id = "2", UserName = "mark@mail.com", FirstName = "Mark", LastName = "Knopfler" };

                Group group = new Group { Id = 1, Name = "Test group 1", OwnerAppUser = groupOwner };

                GroupUser groupUser = new GroupUser { Id = 1, Group = group, AppUser = groupMember };

                context.AppUsers.Add(groupOwner);
                context.AppUsers.Add(groupMember);
                context.Groups.Add(group);
                context.GroupUsers.Add(groupUser);

                context.SaveChanges();
            }
        }

        [Fact]
        public async Task GroupRepository_GetGroupMembersAsync_ShouldNotBeNull()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new GroupRepository(context);

                // Act 
                var groups = await repository.GetGroupMembersAsync(1);
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
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new GroupRepository(context);

                // Act 
                var groups = await repository.GetGroupMembersAsync(33);
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();

                // Assert
                groups.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task GroupRepository_GetUserMembershipInGroup_ShoudReturnFalseWhenUserIdIsWrong()
        {
            using(var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new GroupRepository(context);

                // Act 
                var memberhip = await repository.GetUserMembershipInGroupAsync(1, "1");
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
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new GroupRepository(context);

                // Act 
                var memberhip = await repository.GetUserMembershipInGroupAsync(99, "john@mail.com");
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
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new GroupRepository(context);

                // Act 
                var memberhip = await repository.GetUserMembershipInGroupAsync(1, "john@mail.com");
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
            using (var context = new ApplicationDbContext(_dbContextOptions))
            {
                var repository = new GroupRepository(context);

                // Act 
                var memberhip = await repository.GetUserMembershipInGroupAsync(1, "mark@mail.com");
                // Destroy in-memory database to prevent running multiple instance
                context.Database.EnsureDeleted();


                // Assert 
                memberhip.IsMember.Should().BeTrue();
                memberhip.IsOwner.Should().BeFalse();


            }
        }
    }
}
