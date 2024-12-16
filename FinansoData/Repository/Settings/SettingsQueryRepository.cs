using FinansoData.Data;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Settings
{
    public class SettingsQueryRepository : ISettingsQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(120);

        public SettingsQueryRepository(ApplicationDbContext applicationDbContext, ICacheWrapper cacheWrapper)
        {
            _context = applicationDbContext;
            _cacheWrapper = cacheWrapper;
        }

        public async Task<T> GetSettingsAsync<T>(string key)
        {
            // Check if there is a cache with the key
            if (_cacheWrapper.TryGetValue($"Setting_{key}", out Models.Settings? cacheSetting))
            {
                return (T)Convert.ChangeType(cacheSetting.Value, typeof(T));
            }


            Models.Settings? dbSettingEntity;
            try
            {
                // If there is no cache, check the database
                 dbSettingEntity = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
            }
            catch
            {
                throw new Exception("Error while getting cacheSetting from database");
            }
            


            if (dbSettingEntity == null)
            {
                throw new KeyNotFoundException($"Setting with key '{key}' not found");
            }


            T result;
            try
            {
                result = (T)Convert.ChangeType(dbSettingEntity.Value, typeof(T));
            }
            catch
            {
                throw new Exception("Error while converting cacheSetting value");
            }

            // Set the cache
            _cacheWrapper.Set($"Setting_{key}", dbSettingEntity, _cacheDuration);

            // Return the result
            return result;
        }
    }
}
