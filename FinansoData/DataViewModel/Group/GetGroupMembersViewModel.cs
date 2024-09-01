namespace FinansoData.DataViewModel.Group
{
    public class GetGroupMembersViewModel
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsOwner { get; set; }

    }
}
