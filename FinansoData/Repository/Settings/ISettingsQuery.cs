namespace FinansoData.Repository.Settings
{
    public interface ISettingsQuery
    {
        Task<T> GetSettingsAsync<T>(string key);
    }
}
