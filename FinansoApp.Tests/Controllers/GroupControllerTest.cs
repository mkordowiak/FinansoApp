﻿using AutoMapper;
using FinansoApp.Controllers;
using FinansoApp.ViewModels;
using FinansoData;
using FinansoData.DataViewModel.Group;
using FinansoData.Repository.Group;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FinansoApp.Controllers;
using FinansoData.Repository.Group;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FinansoData.Models;

namespace FinansoApp.Tests.Controllers
{
    public class GroupControllerTest
    {
        private readonly Mock<IGroupQueryRepository> _groupQueryRepositoryMock;
        private readonly Mock<IGroupManagementRepository> _groupManagementRepositoryMock;
        private readonly Mock<IMapper> _mapper;

        public GroupControllerTest()
        {
            _groupQueryRepositoryMock = new Mock<IGroupQueryRepository>();
            _groupManagementRepositoryMock = new Mock<IGroupManagementRepository>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task GroupController_DeleteGroupUser_ShouldRedirectToConfirmPage()
        {
            // Arrange
            _groupQueryRepositoryMock.Setup(x => x.GetUserDeleteInfo(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<DeleteGroupUserViewModel>.Success(new DeleteGroupUserViewModel()));

            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object);

            // Act
            IActionResult groupControllerResult = await groupController.DeleteGroupUser(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            groupControllerResult.Should().BeOfType<ViewResult>("Controller should return view when group user info is received");
        }


        [Fact]
        public async Task GroupController_DeleteGroupUser_ShouldReturnBadRequestIfCantReceiveGroupInfo()
        {
            // Arrange
            _groupQueryRepositoryMock.Setup(x => x.GetUserDeleteInfo(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<DeleteGroupUserViewModel>.Failure(null, ErrorType.ServerError));
            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object);

            // Act
            IActionResult groupControllerResult = await groupController.DeleteGroupUser(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            groupControllerResult.Should().BeOfType<BadRequestResult>("Controller should return bad request when can't receive group info");
        }



        [Fact]
        public async Task GroupController_DeleteGroupUserConfirmed_ShouldReturnUnauthorizedWhenUserIsNotGroupOwner()
        {
            // Arrange
            // Mock repository
            _groupQueryRepositoryMock.Setup(x => x.IsUserGroupOwner(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(false));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            // Create controller object
            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult groupControllerResult = await groupController.DeleteGroupUserConfirmed(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            groupControllerResult.Should().BeOfType<UnauthorizedResult>("Controller should return unauthorized when user is not group owner");
        }

        [Fact]
        public async Task GroupController_DeleteGroupUserConfirmed_ShouldReturnBadRequestWhenIsUserGroupOwnerRepoError()
        {
            // Arrange
            // Mock repository
            _groupQueryRepositoryMock.Setup(x => x.IsUserGroupOwner(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<bool>.Failure(null, ErrorType.ServerError));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            // Create controller object
            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult groupControllerResult = await groupController.DeleteGroupUserConfirmed(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            groupControllerResult.Should().BeOfType<BadRequestResult>("Controller should return bad request when can't check if user is group owner");
        }

        [Fact]
        public async Task GroupController_DeleteGroupUserConfirmed_ShouldReturnBadRequestWhenDeleteGroupUserRepoError()
        {
            // Arrange
            // Mock repository
            _groupQueryRepositoryMock.Setup(x => x.IsUserGroupOwner(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));
            _groupManagementRepositoryMock.Setup(x => x.DeleteGroupUser(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool>.Failure(null, ErrorType.ServerError));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            // Create controller object
            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult groupControllerResult = await groupController.DeleteGroupUserConfirmed(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            groupControllerResult.Should().BeOfType<BadRequestResult>("Controller should return bad request when can't remove user");
        }

        [Fact]
        public async Task GroupController_DeleteGroupUserConfirmed_ShouldReturnRedirectToGroupIndexWhenEverythingIsFine()
        {
            // Arrange
            // Mock repository
            _groupQueryRepositoryMock.Setup(x => x.IsUserGroupOwner(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));
            _groupManagementRepositoryMock.Setup(x => x.DeleteGroupUser(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));


            // Claims Principal Mock
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            // Create controller object
            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult groupControllerResult = await groupController.DeleteGroupUserConfirmed(It.IsAny<int>(), It.IsAny<int>());

            // Assert
            groupControllerResult.Should().BeOfType<RedirectToActionResult>("Controller should return redirect to group index when everything is fine");
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


            _groupManagementRepositoryMock.Setup(x => x.Add(groupName, appUser)).ReturnsAsync(RepositoryResult<bool?>.Failure(null, ErrorType.MaxGroupsLimitReached));


            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
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
        public async Task GroupController_Delete_ShouldReturn404WhenGroupIdIsWrong()
        {
            // Arrange
            _groupQueryRepositoryMock.Setup(x => x.IsGroupExists(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(false));
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();

            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);

            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult deleteGroupResult = await groupController.DeleteGroup(1);

            // Assert
            deleteGroupResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.NotFoundResult>("Controller should return not found when group does not exist");
        }


        [Fact]
        public async Task GroupController_Delete_ShouldReturnUnauthorizedWhenUserIsNotGroupOwner()
        {
            // Arrange

            // Principal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);


            // Repository
            _groupQueryRepositoryMock.Setup(x => x.IsGroupExists(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));
            _groupQueryRepositoryMock.Setup(x => x.IsUserGroupOwner(It.IsAny<int>(), appUser))
                .ReturnsAsync(RepositoryResult<bool>.Success(false));



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult deleteGroupResult = await groupController.DeleteGroup(1);

            // Assert
            deleteGroupResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.UnauthorizedResult>("Controller should return unauthorized when user is not group owner");
        }

        [Fact]
        public async Task GroupController_DeleteGroupConfirmed_ShouldRunDeleteGroupMethod()
        {
            #region Arrange
            // Principal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);


            // Repository
            _groupQueryRepositoryMock.Setup(x => x.IsGroupExists(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));
            _groupQueryRepositoryMock.Setup(x => x.IsUserGroupOwner(It.IsAny<int>(), appUser))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));

            _groupManagementRepositoryMock.Setup(x => x.DeleteGroup(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };
            #endregion


            // Act
            IActionResult deleteGroupResult = await groupController.DeleteGroupConfirmed(It.IsAny<int>());

            // Assert
            _groupManagementRepositoryMock.Verify(x => x.DeleteGroup(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GroupController_Delete_ShouldReturnViewDeleteConfirmedWhenOK()
        {
            // Arrange

            // Principal
            string appUser = "appuser";
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);


            // Repository
            _groupQueryRepositoryMock.Setup(x => x.IsGroupExists(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));
            _groupQueryRepositoryMock.Setup(x => x.IsUserGroupOwner(It.IsAny<int>(), appUser))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));

            _groupManagementRepositoryMock.Setup(x => x.DeleteGroup(It.IsAny<int>()))
                .ReturnsAsync(RepositoryResult<bool>.Success(true));



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);


            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = context.Object
                }
            };

            // Act
            IActionResult deleteGroupResult = await groupController.DeleteGroup(1);

            // Assert
            deleteGroupResult.Should().BeOfType<Microsoft.AspNetCore.Mvc.ViewResult>();

            ViewResult? viewResult = deleteGroupResult as ViewResult;
            viewResult.ViewName.Should().Be("ConfirmGroupDelete");
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

            // Claims Principal Mock
            Mock<ClaimsPrincipal> mockPrincipal = new Mock<ClaimsPrincipal>();
            mockPrincipal.Setup(p => p.Identity.Name).Returns(appUser);
            mockPrincipal.Setup(p => p.Identity.IsAuthenticated).Returns(true);



            Mock<HttpContext> context = new Mock<HttpContext>();
            context.SetupGet(ctx => ctx.User).Returns(mockPrincipal.Object);



            _groupManagementRepositoryMock.Setup(x => x.Add(groupName, appUser))
                .ReturnsAsync(RepositoryResult<bool?>.Success(true));



            GroupController groupController = new GroupController(_mapper.Object, _groupQueryRepositoryMock.Object, _groupManagementRepositoryMock.Object)
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
