namespace FinansoData.Repository.Settings
{
    public interface ISettingsQueryRepository
    {
        Task<T> GetSettingsAsync<T>(string key);
    }
}
