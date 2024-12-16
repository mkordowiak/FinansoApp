namespace FinansoApp.ViewModels
{
    public class GroupMembersViewModel
    {
        public int GroupUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsOwner { get; set; }

        public bool InvitationAccepted { get; set; } = true;
    }

    public class ListMembersViewModel 
    {
        public int GroupId { get; set; }
        public bool IsOwner { get; set; }
        public List<GroupMembersViewModel> GroupMembers { get; set; }
    }

}
