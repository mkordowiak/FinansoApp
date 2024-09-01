﻿using FinansoApp.ViewModels;
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

        public GroupController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
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

            IEnumerable<GetGroupMembersViewModel> data = await _groupRepository.GetUserGroupMembers(id);

            List<GroupMembersViewModel> members = new List<GroupMembersViewModel>();

            return View(members);
        }

        public async Task<IActionResult> DeleteGroup(int id)
        {
            throw new NotImplementedException();
            return RedirectToAction("Index", "Home");
        }
    }
}
