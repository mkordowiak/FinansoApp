using AutoMapper;
using FinansoApp.Controllers;
using FinansoApp.ViewModels;
using FinansoData;
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
        private readonly Mock<IMapper> _mapper;

        public GroupControllerTest()
        {
            _groupRepositoryMock = new Mock<IGroupRepository>();
            _mapper = new Mock<IMapper>();
        }


        [Fact]
        public async Task GroupController_Create_ShoudBeAuthorized()
        {
            // Arrange
            MethodInfo? httpPostMethod = typeof(GroupController).GetMethod(nameof(GroupController.Create), new[] { typeof(GroupCreateViewModel) });
            MethodInfo? httpGetMethod = typeof(GroupController).GetMethod(nameof(GroupController.Create), Type.EmptyTypes);

            // Act
            AuthorizeAttribute? httpPostAuthorizeAttribute = httpPostMethod.GetCustomAttribute<AuthorizeAttribute>();
            AuthorizeAttribute? httpGetAuthorizeAttribute = httpGetMethod.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            httpPostAuthorizeAttribute.Should().NotBeNull("this method should be protected by authorization");
            httpGetAuthorizeAttribute.Should().NotBeNull("this method shoud be protected by authorization");
        }

        [Fact]
        public async Task GroupController_Index_ShouldBeAuthorized()
        {
            // Arrange
            MethodInfo? httpGetMethod = typeof(GroupController).GetMethod(nameof(GroupController.Index), Type.EmptyTypes);

            // Act
            AuthorizeAttribute? httpGetAuthorizeAttribute = httpGetMethod.GetCustomAttribute<AuthorizeAttribute>();

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
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            _groupRepositoryMock.Setup(x => x.Add(groupName, appUser)).ReturnsAsync(RepositoryResult<bool?>.Failure(null, ErrorType.MaxGroupsLimitReached));


            GroupController groupController = new GroupController(_groupRepositoryMock.Object, _mapper.Object)
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
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);



            _groupRepositoryMock.Setup(x => x.Add(groupName, appUser))
                .ReturnsAsync(RepositoryResult<bool?>.Success(true));



            GroupController groupController = new GroupController(_groupRepositoryMock.Object, _mapper.Object)
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
