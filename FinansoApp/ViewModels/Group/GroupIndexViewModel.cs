using FinansoData.DataViewModel.Group;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace FinansoApp.ViewModels.Group
{
    public class GroupIndexViewModel
    {
        public List<GetUserGroupsViewModel>? Groups { get; set; }
        public int GroupInvitations { get; set; }
    }
}
