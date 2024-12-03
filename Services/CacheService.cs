using api_client_with_redis_cache.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace api_client_with_redis_cache.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<string> GetOrAddAsync(string key, Func<Task<dynamic>> dataSource, TimeSpan? expirationTime = null)
        {
            string cachedData = await _distributedCache.GetStringAsync(key);

            if (cachedData != null)
            {
                return cachedData;
            }

            try
            {
                // Fetching from the data source
                var data = await dataSource();
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromMinutes(30)  // Default caching for 30 minutes
                };

                string dataStringJson = JsonSerializer.Serialize<dynamic>(data);

                await _distributedCache.SetStringAsync(key, dataStringJson, options);
                return dataStringJson;
            }
            catch (Exception)
            {
                // If fetching data source throws error and stale data is acceptable, return cached data
                return cachedData;
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
                return true;
            }
            catch (Exception e) { 
                return false;
            }
        }
    }
}
