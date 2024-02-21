using FinansoApp.Controllers;
using FinansoApp.ViewModels;
using FinansoData.Repository;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using System.Security.Claims;

namespace FinansoApp.Tests.Controllers
{
    public class GroupControllerTest
    {
        private readonly Mock<IGroupRepository> _groupRepositoryMock;

        public GroupControllerTest()
        {
            _groupRepositoryMock = new Mock<IGroupRepository>();
        }

        [Fact]
        public async Task GroupController_Create_ShoudBeAuthorized()
        {
            // Arrange
            var httpPostMethod = typeof(GroupController).GetMethod(nameof(GroupController.Create), new[] { typeof(GroupCreateViewModel) });
            var httpGetMethod = typeof(GroupController).GetMethod(nameof(GroupController.Create), Type.EmptyTypes);

            // Act
            var httpPostAuthorizeAttribute = httpPostMethod.GetCustomAttribute<AuthorizeAttribute>();
            var httpGetAuthorizeAttribute = httpGetMethod.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            httpPostAuthorizeAttribute.Should().NotBeNull("this method should be protected by authorization");
            httpGetAuthorizeAttribute.Should().NotBeNull("this method shoud be protected by authorization");
        }

        [Fact]
        public async Task GroupController_Index_ShouldBeAuthorized()
        {
            // Arrange
            var httpGetMethod = typeof(GroupController).GetMethod(nameof(GroupController.Index), Type.EmptyTypes);

            // Act
            var httpGetAuthorizeAttribute = httpGetMethod.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            httpGetAuthorizeAttribute.Should().NotBeNull("this method shoud be protected by authorization");
        }


        [Fact]
        public async Task GroupController_Create_ShoudShowErrorWhenTooManyGroups()
        {
            // Arrange
            string groupName = "group name";
            string appUser = "appuser";
            GroupCreateViewModel groupCreateVMInput = new GroupCreateViewModel
            {
                Name = groupName
            };

            //
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser); 
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);



            var context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);



            _groupRepositoryMock.Setup(x => x.Add(groupName, appUser))
                .ReturnsAsync(false);
            _groupRepositoryMock.Setup(x => x.Error.MaxGroupsLimitReached)
                .Returns(true);

            GroupController groupController = new GroupController(_groupRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };
            


            // Act
            IActionResult createGroupResult = await groupController.Create(groupCreateVMInput);

            // Assert
            createGroupResult.Should().BeOfType<ViewResult>();
            GroupCreateViewModel? groupCreateVMResult = (createGroupResult as ViewResult).Model as GroupCreateViewModel;
            groupCreateVMResult.Error.MaxGroupsLimitReached.Should().Be(true);
        }



        [Fact]
        public async Task GroupController_Create_ShoudRedirectToHomeWhenOk()
        {
            // Arrange
            string groupName = "group name";
            string appUser = "appuser";
            GroupCreateViewModel groupCreateVMInput = new GroupCreateViewModel
            {
                Name = groupName
            };

            //
            var mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);



            var context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);



            _groupRepositoryMock.Setup(x => x.Add(groupName, appUser))
                .ReturnsAsync(true);
            _groupRepositoryMock.Setup(x => x.Error.MaxGroupsLimitReached)
                .Returns(false);

            GroupController groupController = new GroupController(_groupRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };


            // Act
            IActionResult createGroupResult = await groupController.Create(groupCreateVMInput);

            // Assert
            createGroupResult.Should().BeOfType<RedirectToActionResult>("Method shoud redirect when ok");
        }
    }
}
