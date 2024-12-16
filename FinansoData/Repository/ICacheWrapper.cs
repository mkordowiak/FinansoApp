namespace FinansoData.Repository
{
    public interface ICacheWrapper
    {
        void Set(string key, object value, TimeSpan cacheDuration);
        bool TryGetValue<T>(string key, out T value);
        void Remove(string key);
    }

}