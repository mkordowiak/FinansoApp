using FinansoData.Data;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Settings
{
    public class SettingsQuery : ISettingsQuery
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(120);

        public SettingsQuery(ApplicationDbContext applicationDbContext, ICacheWrapper cacheWrapper)
        {
            _applicationDbContext = applicationDbContext;
            _cacheWrapper = cacheWrapper;
        }

        public async Task<T> GetSettingsAsync<T>(string key)
        {
            if (_cacheWrapper.TryGetValue(key, out T value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            Models.Settings? setting = await _applicationDbContext.Settings.FirstOrDefaultAsync(s => s.Key == key);

            if (setting == null)
            {
                throw new KeyNotFoundException($"Setting with key '{key}' not found");
            }


            T result = (T)Convert.ChangeType(setting.Value, typeof(T));
            _cacheWrapper.Set(key, result, _cacheDuration);
            return result;
        }
    }
}
