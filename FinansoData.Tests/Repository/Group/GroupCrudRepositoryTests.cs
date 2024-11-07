using FinansoData.Data;
using FinansoData.Repository.Group;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Tests.Repository.Group
{

    public class GroupCrudRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public GroupCrudRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


            }
        }

        [Fact]
        public void Add_ValidGroup_ReturnsTrue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                Models.Group group = new Models.Group
                {
                    Name = "Group name",
                    OwnerAppUser = new Models.AppUser(),
                    Created = DateTime.Now
                };

                // Act 
                bool result = groupCrudRepository.Add(group);

                // Assert
                Assert.True(result);
                Assert.Contains(group, context.Groups);
            }
        }

        [Fact]
        public void Delete_ValidGroup_ReturnsTrue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                Models.Group group = new Models.Group
                {
                    Name = "Group name",
                    OwnerAppUser = new Models.AppUser(),
                    Created = DateTime.Now
                };
                context.Groups.Add(group);
                context.SaveChanges();

                // Act
                bool result = groupCrudRepository.Delete(group);

                // Assert
                Assert.True(result);
                Assert.DoesNotContain(group, context.Groups);
            }
        }

        [Fact]
        public void Update_ValidGroup_ReturnsTrue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                Models.Group group = new Models.Group
                {
                    Name = "Group name",
                    OwnerAppUser = new Models.AppUser(),
                    Created = DateTime.Now
                };
                context.Groups.Add(group);
                context.SaveChanges();

                // Act
                group.Name = "Updated group name";
                bool result = groupCrudRepository.Update(group);

                // Assert
                Assert.True(result);
                Assert.Contains(group, context.Groups);
                Assert.Equal("Updated group name", group.Name);
            }
        }

        [Fact]
        public async Task UpdateAsync_ValidGroup_ReturnsTrue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                GroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                Models.Group group = new Models.Group
                {
                    Name = "Group name",
                    OwnerAppUser = new Models.AppUser(),
                    Created = DateTime.Now
                };
                context.Groups.Add(group);
                await context.SaveChangesAsync();

                // Act
                group.Name = "Updated group name";
                bool result = await groupCrudRepository.UpdateAsync(group);

                // Assert
                Assert.True(result);
                Assert.Contains(group, context.Groups);
                Assert.Equal("Updated group name", group.Name);
            }
        }
    }
}


