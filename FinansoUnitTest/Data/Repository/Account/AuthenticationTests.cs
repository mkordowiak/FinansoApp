using FinansoData.Data;
using FinansoData.Models;
using FinansoData.Repository.Account;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinansoUnitTest.Data.Repository.Account
{
    public class AuthenticationTests
    {
        [Fact]
        public async Task GetUserAsync_UserExists_ReturnsSuccessResult()
        {
            // Arrange
            var username = "testuser";
            var user = new AppUser { UserName = username };

            // Mocking the DbSet as IQueryable
            var mockSet = new Mock<DbSet<AppUser>>();
            var users = new List<AppUser> { user }.AsQueryable();

            mockSet.As<IQueryable<AppUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.AppUsers).Returns(mockSet.Object);

            var userManager = Mock.Of<UserManager<AppUser>>();
            var repo = new Authentication(mockContext.Object, userManager);

            // Act
            var result = await repo.GetUserAsync(username);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(user);
        }


    }
}
