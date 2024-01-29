using FinansoApp.ViewModels;
using FinansoData.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinansoData.DataViewModel;
using FinansoData.DataViewModel.Group;

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
            IEnumerable< GetUserGroupsViewModel> data = await _groupRepository.GetUserGroups(User.Identity.Name);
            return View();
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
                && _groupRepository.Error.Any(x => x.Key == "MaxGroupsLimitReached"))
            {
                TempData["MaxGroupsLimitReached"] = true;
                return View(groupCreateViewModel);
            }

            if (createGroup == false)
            {
                TempData["InternalError"] = true;
                return View(groupCreateViewModel);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
