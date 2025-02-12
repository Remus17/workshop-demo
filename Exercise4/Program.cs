using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("redisserver:6379"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    WeatherForecast[] forecast = GetForecast();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/weatherforecastwithcache", async ([FromServices] IConnectionMultiplexer connection) =>
{
    var db = connection.GetDatabase();
    string? forecast = await db.StringGetAsync("weatherforecast");
    if (forecast == null)
    {
        var forecastData = GetForecast();
        await db.StringSetAsync("weatherforecast", JsonSerializer.Serialize(forecastData), TimeSpan.FromMinutes(1));
        return Results.Ok(forecastData);
    }
    var cachedData = JsonSerializer.Deserialize<WeatherForecast[]>(forecast);
    return Results.Ok(cachedData);
})
.WithName("GetWeatherForecastWithCache");

app.Run();

WeatherForecast[] GetForecast()
{
    return Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)],
            app.Environment.EnvironmentName,
            app.Configuration.GetValue<string>("MyVariable")
        ))
        .ToArray();
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string Environment, string? MyVariable)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    #if (DEBUG)
    public int DebugProperty { get; set; }
    #else 
    public int ProductionProperty { get; set; }
    #endif
}

