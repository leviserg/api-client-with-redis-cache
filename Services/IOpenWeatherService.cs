namespace api_client_with_redis_cache.Services
{
    public interface IOpenWeatherService
    {
        Task<dynamic> GetWeatherAsync(string city);
    }
}
