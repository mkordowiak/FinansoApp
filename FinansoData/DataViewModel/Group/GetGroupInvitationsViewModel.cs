namespace FinansoData.DataViewModel.Group
{
    public class GetGroupInvitationsViewModel
    {
        public int GroupUserId { get; set; }
        public string GroupName { get; set; }
        public string? GroupOwnerFirstName { get; set; }
        public string? GroupOwnerLastName { get; set; }
        public int GroupMembersNum { get; set; }
    }
}
