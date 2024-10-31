namespace FinansoData.Repository.Group
{
    public interface IGroupManagementRepository
    {
        Task<RepositoryResult<bool?>> Add(string groupName, string appUser);
    }
}
