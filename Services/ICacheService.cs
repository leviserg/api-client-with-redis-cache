namespace api_client_with_redis_cache.Services
{
    public interface ICacheService
    {
        Task<string> GetOrAddAsync(string key, Func<Task<dynamic>> dataSource, TimeSpan? expirationTime = null);
        Task<bool> RemoveAsync(string key);
    }
}
