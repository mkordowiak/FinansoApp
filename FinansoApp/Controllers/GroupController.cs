using AutoMapper;
using FinansoApp.ViewModels;
using FinansoData.DataViewModel.Group;
using FinansoData.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinansoApp.Controllers
{
    public class GroupController : Controller
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public GroupController(IGroupRepository groupRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            FinansoData.RepositoryResult<IEnumerable<GetUserGroupsViewModel>?> data = await _groupRepository.GetUserGroups(User.Identity.Name);
            return View(data.Value);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(GroupCreateViewModel groupCreateViewModel)
        {
            FinansoData.RepositoryResult<bool?> createGroup = await _groupRepository.Add(groupCreateViewModel.Name, User.Identity.Name);


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


        [Authorize]
        public async Task<IActionResult> EditMembers(int id)
        {
            // check loggedin user access
            FinansoData.RepositoryResult<GetUserMembershipInGroupViewModel> groupMemberInfo = await _groupRepository.GetUserMembershipInGroupAsync(id, User.Identity.Name);

            if (groupMemberInfo.Value.IsMember == false)
            {
                return RedirectToAction("Index", "Home");
            }


            // get data
            FinansoData.RepositoryResult<IEnumerable<GetGroupMembersViewModel>> data = await _groupRepository.GetGroupMembersAsync(id);
            List<GroupMembersViewModel> members = _mapper.Map<List<GroupMembersViewModel>>(data.Value);

            ListMembersViewModel listMembersViewModel = new ListMembersViewModel
            {
                IsOwner = groupMemberInfo.Value.IsOwner,
                GroupMembers = members
            };

            return View(listMembersViewModel);
        }

        public async Task<IActionResult> DeleteGroup(int id)
        {
            throw new NotImplementedException();
            return RedirectToAction("Index", "Home");
        }
    }
}
