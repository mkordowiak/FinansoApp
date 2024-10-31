using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinansoData.Tests.Repository.Group
{
    public class GroupManagmentRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private AppUser _groupOwner;
        private AppUser _groupMember;
        private FinansoData.Models.Group _group;
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
        public async Task GroupRepository_Add_ShoudBeTrue()
        {
            // Arrange
            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupManagementRepository groupRepository = new GroupManagementRepository(context, groupCrudRepository);
                
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
                IGroupCrudRepository groupCrudRepository = new GroupCrudRepository(context);
                GroupManagementRepository groupRepository = new GroupManagementRepository(context, groupCrudRepository);

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


    }
}
