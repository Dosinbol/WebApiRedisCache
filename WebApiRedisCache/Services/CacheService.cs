using StackExchange.Redis;
using System.Text.Json;
using WebApiRedisCache.Services.Interfaces;

namespace WebApiRedisCache.Services
{
    public class CacheService : ICacheService
    {
        private IDatabase _cacheDb;
        private TimeSpan timeSpan = TimeSpan.FromDays(1);
        public CacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDb = redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public object RemoveData(string key)
        {
            var exist = _cacheDb.KeyExists(key);
            if (exist)
            {
                return _cacheDb.KeyDelete(key);
            }
            return false;
        }

        public bool SetData<T>(string key, T value)
        {
            var isSet = _cacheDb.StringSet(key, JsonSerializer.Serialize(value), timeSpan);
            return isSet;
        }
    }
}
