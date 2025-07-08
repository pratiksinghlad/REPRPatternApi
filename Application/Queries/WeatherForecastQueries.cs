using MediatR;
using REPRPatternApi.Models;

namespace REPRPatternApi.Controllers;

/// <summary>
/// Query to get weather forecast
/// </summary>
public class GetWeatherForecastQuery : IRequest<IEnumerable<WeatherForecast>>
{
    /// <summary>
    /// Number of days to forecast
    /// </summary>
    public int Days { get; set; } = 5;
}

/// <summary>
/// Query to get weather forecast for a specific date
/// </summary>
public class GetWeatherForecastByDateQuery : IRequest<WeatherForecast>
{
    /// <summary>
    /// The date to get forecast for
    /// </summary>
    public DateOnly Date { get; set; }
}

/// <summary>
/// Handler for weather forecast queries
/// </summary>
public class WeatherForecastQueryHandler : 
    IRequestHandler<GetWeatherForecastQuery, IEnumerable<WeatherForecast>>,
    IRequestHandler<GetWeatherForecastByDateQuery, WeatherForecast>
{
    private readonly ILogger<WeatherForecastQueryHandler> _logger;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    /// <summary>
    /// Initializes a new instance of the WeatherForecastQueryHandler
    /// </summary>
    /// <param name="logger">Logger instance</param>
    public WeatherForecastQueryHandler(ILogger<WeatherForecastQueryHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetWeatherForecastQuery
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weather forecast data</returns>
    public async Task<IEnumerable<WeatherForecast>> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling weather forecast request for {Days} days", request.Days);
        
        await Task.Delay(10, cancellationToken); // Simulate some async work
        
        return Enumerable.Range(1, request.Days).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        });
    }

    /// <summary>
    /// Handles the GetWeatherForecastByDateQuery
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weather forecast for the specified date</returns>
    public async Task<WeatherForecast> Handle(GetWeatherForecastByDateQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling weather forecast request for date {Date}", request.Date);
        
        await Task.Delay(10, cancellationToken); // Simulate some async work
        
        return new WeatherForecast
        {
            Date = request.Date,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        };
    }
}