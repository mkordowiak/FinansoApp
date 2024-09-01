using AutoMapper;
using FinansoApp.ViewModels;
using FinansoData.DataViewModel.Group;
using FinansoData.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

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
            IEnumerable<GetUserGroupsViewModel> data = await _groupRepository.GetUserGroups(User.Identity.Name);
            return View(data);
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
            bool createGroup = await _groupRepository.Add(groupCreateViewModel.Name, User.Identity.Name);


            if (createGroup == false
                && _groupRepository.Error.MaxGroupsLimitReached)
            {
                groupCreateViewModel.Error.MaxGroupsLimitReached = true;
                return View(groupCreateViewModel);
            }

            if (createGroup == false)
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
            var groupMemberInfo = await _groupRepository.GetUserMembershipInGroup(id, User.Identity.Name);

            if (groupMemberInfo.IsMember == false)
            {
                return RedirectToAction("Index", "Home");
            }


            // get data
            IEnumerable<GetGroupMembersViewModel> data = await _groupRepository.GetUserGroupMembers(id);
            List<GroupMembersViewModel> members = _mapper.Map<List<GroupMembersViewModel>>(data);

            ListMembersViewModel listMembersViewModel = new ListMembersViewModel
            {
                IsOwner = groupMemberInfo.IsOwner,
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
