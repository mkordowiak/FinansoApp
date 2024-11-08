namespace FinansoApp.ViewModels
{
    public class GroupMembersViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsOwner { get; set; }
    }

    public class ListMembersViewModel 
    {
        public bool IsOwner { get; set; }
        public List<GroupMembersViewModel> GroupMembers { get; set; }
    }

}
