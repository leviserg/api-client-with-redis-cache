using api_client_with_redis_cache.Models;
using api_client_with_redis_cache.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace api_client_with_redis_cache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenWeatherController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IOpenWeatherService _openWeatherService;

        public OpenWeatherController(IOpenWeatherService openWeatherService, ICacheService cacheService)
        {
            _openWeatherService = openWeatherService;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<ActionResult<CustomWeatherModel>> GetWeather([FromQuery] string city)
        {
            try
            {
                string cacheKey = $"openweather-cache-{city}";

                var result = await _cacheService.GetOrAddAsync(
                    key: cacheKey,
                    dataSource: async () => await _openWeatherService.GetWeatherAsync(city),
                    expirationTime: TimeSpan.FromMinutes(5)
                );

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var model = JsonSerializer.Deserialize<ExternalWeatherModel>(result, options);

                return Ok(model?.ToCustomWeatherModel());
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }

        }
    }
}
