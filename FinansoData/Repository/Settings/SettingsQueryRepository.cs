using FinansoData.Data;
using FinansoData.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Settings
{
    public class SettingsQueryRepository : ISettingsQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(120);
        private readonly string _cacheClassName;

        public SettingsQueryRepository(ApplicationDbContext applicationDbContext, ICacheWrapper cacheWrapper)
        {
            _context = applicationDbContext;
            _cacheWrapper = cacheWrapper;
            _cacheClassName = this.GetType().Name;
        }

        public async Task<T> GetSettingsAsync<T>(string key)
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}_{key}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out Models.Settings? cacheSetting))
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
            _cacheWrapper.Set(cacheDataKey, dbSettingEntity, _cacheDuration);

            // Return the result
            return result;
        }
    }
}
