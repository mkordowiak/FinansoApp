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

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public void Set(string key, object value, TimeSpan cacheDuration)
        {
#if DEBUG
            return;
#else
            _memoryCache.Set(key, value, cacheDuration);
#endif
        }

        public bool TryGetValue<T>(string key, out T value)
        {
#if DEBUG
            value = default;
            return false;

#else
            return _memoryCache.TryGetValue(key, out value);
#endif
        }
    }
}
