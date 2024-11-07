using AutoMapper;
using FinansoApp.ViewModels;
using FinansoData.DataViewModel.Group;
using FinansoData.Repository.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Controllers
{
    public class GroupController : Controller
    {
        //private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        private readonly IGroupQueryRepository _groupQueryRepository;
        private readonly IGroupManagementRepository _groupManagementRepository;

        public GroupController(IMapper mapper, IGroupQueryRepository groupQueryRepository, IGroupManagementRepository groupManagementRepository)
        {
            _mapper = mapper;
            _groupQueryRepository = groupQueryRepository;
            _groupManagementRepository = groupManagementRepository;
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
            FinansoData.RepositoryResult<GetUserMembershipInGroupViewModel> groupMemberInfo = await _groupQueryRepository.GetUserMembershipInGroupAsync(id, User.Identity.Name);

            if (groupMemberInfo.Value.IsMember == false)
            {
                return RedirectToAction("Index", "Home");
            }


            // get data
            FinansoData.RepositoryResult<IEnumerable<GetGroupMembersViewModel>> data = await _groupQueryRepository.GetGroupMembersAsync(id);
            List<GroupMembersViewModel> members = _mapper.Map<List<GroupMembersViewModel>>(data.Value);

            ListMembersViewModel listMembersViewModel = new ListMembersViewModel
            {
                IsOwner = groupMemberInfo.Value.IsOwner,
                GroupMembers = members
            };

            return View(listMembersViewModel);
        }


        /// <summary>
        /// Delete group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [Authorize]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            // check if groups exits
            FinansoData.RepositoryResult<bool> isGroupExists = await _groupQueryRepository.IsGroupExists(id);

            if (!isGroupExists.IsSuccess)
            {
                return BadRequest();
            }

            if (isGroupExists.Value == false)
            {
                return NotFound();
            }


            // check if user is group owner
            FinansoData.RepositoryResult<bool> userGroupOwner = await _groupQueryRepository.IsUserGroupOwner(id, User.Identity.Name);

            if (!userGroupOwner.IsSuccess)
            {
                return BadRequest();
            }

            if (userGroupOwner.Value == false)
            {
                return Unauthorized();
            }


            FinansoData.RepositoryResult<bool> result = await _groupManagementRepository.DeleteGroup(id);

            if (!result.IsSuccess)
            {
                return BadRequest();
            }


            return RedirectToAction("Index", "Group");
        }
    }
}
