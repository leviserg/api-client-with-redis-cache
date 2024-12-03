using api_client_with_redis_cache.Models;
using System.Net;

namespace api_client_with_redis_cache.Services
{
    public class OpenWeatherService : IOpenWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenWeatherService> _logger;

        public OpenWeatherService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<OpenWeatherService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<dynamic> GetWeatherAsync(string city)
        {

            string url = _configuration.GetValue<string>("OpenWeatherUrl") ?? string.Empty;

            var apiKey = Environment.GetEnvironmentVariable("OpenWeatherApiKey") ?? string.Empty;

            url = string.Format(url, city, apiKey);


            var weatherResponse = await _httpClient.GetAsync(url);

            if (weatherResponse.StatusCode == HttpStatusCode.OK)
            {
                return await weatherResponse.Content.ReadFromJsonAsync<dynamic?>() ?? new { };
            }

            if (weatherResponse.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("Requested city not found");
            }

            throw new Exception("OpenWeather api failed");

        }
    }
}
