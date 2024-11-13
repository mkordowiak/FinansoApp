using AutoMapper;
using FinansoApp.ViewModels;
using FinansoData.DataViewModel.Group;
using FinansoData.Models;
using FinansoData.Repository.Account;
using FinansoData.Repository.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Controllers
{
    public class GroupController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IGroupQueryRepository _groupQueryRepository;
        private readonly IGroupManagementRepository _groupManagementRepository;
        private readonly IGroupUsersQueryRepository _groupUsersQuery;
        private readonly IGroupUsersManagementRepository _groupUsersManagementRepository;
        private readonly IUserQuery _userQuery;

        public GroupController(
            IMapper mapper, 
            IGroupQueryRepository groupQueryRepository, 
            IGroupManagementRepository groupManagementRepository, 
            IGroupUsersQueryRepository groupUsersQuery, 
            IGroupUsersManagementRepository groupUsersManagementRepository, 
            IUserQuery userQuery)
        {
            _mapper = mapper;
            _groupQueryRepository = groupQueryRepository;
            _groupManagementRepository = groupManagementRepository;
            _groupUsersQuery = groupUsersQuery;
            _groupUsersManagementRepository = groupUsersManagementRepository;
            _userQuery = userQuery;
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            FinansoData.RepositoryResult<IEnumerable<GetUserGroupsViewModel>?> data = await _groupQueryRepository.GetUserGroups(User.Identity.Name);
            return View(data.Value);
        }

        /// <summary>
        /// Create group page
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Greate method
        /// </summary>
        /// <param name="groupCreateViewModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(GroupCreateViewModel groupCreateViewModel)
        {
            FinansoData.RepositoryResult<bool?> createGroup = await _groupManagementRepository.Add(groupCreateViewModel.Name, User.Identity.Name);


            if (createGroup.IsSuccess == false
                && createGroup.ErrorType == FinansoData.ErrorType.MaxGroupsLimitReached)
            {
                groupCreateViewModel.Error.MaxGroupsLimitReached = true;
                return View(groupCreateViewModel);
            }

            if (createGroup.IsSuccess == false)
            {
                groupCreateViewModel.Error.InternalError = true;
                return View(groupCreateViewModel);
            }
            return RedirectToAction("Index", "Group");
        }


        /// <summary>
        /// Group edit page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> EditMembers(int id)
        {
            // check logged in user access
            FinansoData.RepositoryResult<GetUserMembershipInGroupViewModel> groupMemberInfo = await _groupUsersQuery.GetUserMembershipInGroupAsync(id, User.Identity.Name);

            if (groupMemberInfo.Value.IsMember == false)
            {
                return RedirectToAction("Index", "Home");
            }


            // get data
            FinansoData.RepositoryResult<IEnumerable<GetGroupMembersViewModel>> data = await _groupUsersQuery.GetGroupMembersAsync(id);
            List<GroupMembersViewModel> members = _mapper.Map<List<GroupMembersViewModel>>(data.Value);

            ListMembersViewModel listMembersViewModel = new ListMembersViewModel
            {
                IsOwner = groupMemberInfo.Value.IsOwner,
                GroupMembers = members,
                GroupId = id
            };

            return View(listMembersViewModel);
        }


        /// <summary>
        /// Delete group confirmation page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            // Check if group exists
            FinansoData.RepositoryResult<bool> isGroupExists = await _groupQueryRepository.IsGroupExists(id);

            if (!isGroupExists.IsSuccess)
            {
                return BadRequest();
            }

            if (isGroupExists.Value == false)
            {
                return NotFound();
            }

            // Check if user is group owner
            FinansoData.RepositoryResult<bool> userGroupOwner = await _groupUsersQuery.IsUserGroupOwner(id, User.Identity.Name);

            if (!userGroupOwner.IsSuccess)
            {
                return BadRequest();
            }

            if (userGroupOwner.Value == false)
            {
                return Unauthorized();
            }

            // Show confirmation view
            return View("ConfirmGroupDelete", id);
        }

        /// <summary>
        /// Delete group after confirmation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteGroupConfirmed(int id)
        {
            // Check if group exists
            FinansoData.RepositoryResult<bool> isGroupExists = await _groupQueryRepository.IsGroupExists(id);

            if (!isGroupExists.IsSuccess)
            {
                return BadRequest();
            }

            if (isGroupExists.Value == false)
            {
                return NotFound();
            }

            // Check if user is group owner
            FinansoData.RepositoryResult<bool> userGroupOwner = await _groupUsersQuery.IsUserGroupOwner(id, User.Identity.Name);

            if (!userGroupOwner.IsSuccess)
            {
                return BadRequest();
            }

            if (userGroupOwner.Value == false)
            {
                return Unauthorized();
            }

            // Delete the group
            FinansoData.RepositoryResult<bool> result = await _groupManagementRepository.DeleteGroup(id);

            if (!result.IsSuccess)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Group");
        }

        [Authorize]
        public async Task<IActionResult> DeleteGroupUser(int id, int groupId)
        {
            FinansoData.RepositoryResult<DeleteGroupUserViewModel> data = await _groupUsersQuery.GetUserDeleteInfo(id);

            if (!data.IsSuccess)
            {
                return BadRequest();
            }


            ConfirmGroupUserDeleteViewModel vm = _mapper.Map<ConfirmGroupUserDeleteViewModel>(data.Value);

            // Show confirmation view
            return View("ConfirmGroupUserDelete", vm);
        }

        [Authorize]
        public async Task<IActionResult> DeleteGroupUserConfirmed(int groupUserId, int groupId)
        {
            // Check if user is group owner
            FinansoData.RepositoryResult<bool> userGroupOwner = await _groupUsersQuery.IsUserGroupOwner(groupId, User.Identity.Name);

            // Return bad request if something went wrong
            if (!userGroupOwner.IsSuccess)
            {
                return BadRequest();
            }

            // Return unauthorized if user is not group owner
            if (userGroupOwner.Value == false)
            {
                return Unauthorized();
            }

            // Delete the user from group
            FinansoData.RepositoryResult<bool> deleteGroupUserResult = await _groupUsersManagementRepository.RemoveUserFromGroup(groupUserId);

            // Return bad request if something went wrong
            if (!deleteGroupUserResult.IsSuccess)
            {
                return BadRequest();
            }

            // Redirect to group page
            return RedirectToAction("Index", "Group");
        }

        [Authorize]
        public async Task<IActionResult> AddGroupUser(int id)
        {
            // Check if user is group owner
            FinansoData.RepositoryResult<bool> userGroupOwner = await _groupUsersQuery.IsUserGroupOwner(id, User.Identity.Name);

            // Return bad request if something went wrong
            if (!userGroupOwner.IsSuccess)
            {
                return BadRequest();
            }

            // Return unauthorized if user is not group owner
            if (userGroupOwner.Value == false)
            {
                return Unauthorized();
            }

            AddGroupUserViemModel vm = new AddGroupUserViemModel
            {
                GroupId = id
            };

            return View("AddGroupUser", vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddGroupUser(AddGroupUserViemModel model)
        {
            // Check if user is trying to add himself
            if (model.UserName == User.Identity.Name)
            {
                return View("AddGroupUser", new AddGroupUserViemModel
                {
                    GroupId = model.GroupId,
                    UserName = model.UserName,
                    Error = new AddGroupUserViemModel.AddGroupUserErrorInfo { CantAddYourself = true }
                });
            }

            // Check if user is group owner
            FinansoData.RepositoryResult<bool> userGroupOwner = await _groupUsersQuery.IsUserGroupOwner(model.GroupId, User.Identity.Name);

            // Return bad request if something went wrong
            if (!userGroupOwner.IsSuccess)
            {
                return View("AddGroupUser", new AddGroupUserViemModel
                {
                    GroupId = model.GroupId,
                    UserName = model.UserName,
                    Error = new AddGroupUserViemModel.AddGroupUserErrorInfo { InternalError = true }
                });
            }

            // Return unauthorized if user is not group owner
            if (userGroupOwner.Value == false)
            {
                return Unauthorized();
            }


            // Check if user is already in group
            FinansoData.RepositoryResult<GetUserMembershipInGroupViewModel> userInGroupMembership = await _groupUsersQuery.GetUserMembershipInGroupAsync(model.GroupId, model.UserName);

            // Return bad request if something went wrong
            if (!userInGroupMembership.IsSuccess)
            {
                return View("AddGroupUser", new AddGroupUserViemModel
                {
                    GroupId = model.GroupId,
                    UserName = model.UserName,
                    Error = new AddGroupUserViemModel.AddGroupUserErrorInfo { InternalError = true }
                });
            }

            if (userInGroupMembership.Value.IsMember)
            {
                return View("AddGroupUser", new AddGroupUserViemModel
                {
                    GroupId = model.GroupId,
                    UserName = model.UserName,
                    Error = new AddGroupUserViemModel.AddGroupUserErrorInfo { UserAlreadyInGroup = true }
                });
            }



            // Get user by email
            FinansoData.RepositoryResult<AppUser?> getUserByEmailResult = await _userQuery.GetUserByEmail(model.UserName);

            if (!getUserByEmailResult.IsSuccess)
            {
                return View("AddGroupUser", new AddGroupUserViemModel
                {
                    GroupId = model.GroupId,
                    UserName = model.UserName,
                    Error = new AddGroupUserViemModel.AddGroupUserErrorInfo { InternalError = true }
                });
            }

            if (getUserByEmailResult.Value == null)
            {
                return View("AddGroupUser", new AddGroupUserViemModel
                {
                    GroupId = model.GroupId,
                    UserName = model.UserName,
                    Error = new AddGroupUserViemModel.AddGroupUserErrorInfo { UserNotFound = true }
                });
            }

            // Add user to group
            FinansoData.RepositoryResult<bool> result = await _groupUsersManagementRepository.AddUserToGroup(model.GroupId, getUserByEmailResult.Value);

            // Return bad request if something went wrong
            if (!result.IsSuccess)
            {
                return BadRequest();
            }

            // Redirect to group page
            return RedirectToAction("EditMembers", "Group", new { id = model.GroupId });
        }
    }
}
