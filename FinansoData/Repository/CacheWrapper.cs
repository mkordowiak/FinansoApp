using Microsoft.Extensions.Caching.Memory;

namespace FinansoData.Repository
{
    public class CacheWrapper : ICacheWrapper
    {
        private readonly IMemoryCache _memoryCache;

        public CacheWrapper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set(string key, object value, TimeSpan cacheDuration)
        {
            _memoryCache.Set(key, value, cacheDuration);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }
    }
}
