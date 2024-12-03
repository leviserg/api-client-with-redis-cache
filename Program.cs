using api_client_with_redis_cache.Services;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// start Redis: > docker run --name redislocal -d -p 6379:6379 redis/redis-stack-server:6.2.6-v17
// check status with Redis client


// Add services to the container.
var configuration = builder.Configuration;
builder.Services.AddSingleton(configuration);


builder.Services.AddStackExchangeRedisCache(options =>
{
    configuration.GetSection("RedisCacheOptions").Bind(options);
});

builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddScoped<IOpenWeatherService, OpenWeatherService>();

builder.Services.AddHttpClient<OpenWeatherService>("OpenWeatherClient")
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.Delay = TimeSpan.FromSeconds(5);
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.BackoffType = DelayBackoffType.Exponential;
        options.Retry.UseJitter = true;
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(11);
    }
);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
