using MediatR;
using Microsoft.AspNetCore.Mvc;
using REPRPatternApi.Models;
using System.ComponentModel.DataAnnotations;
using Asp.Versioning;

namespace REPRPatternApi.Controllers;

/// <summary>
/// Weather forecast controller demonstrating XML/JSON content negotiation
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json", "application/xml")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    /// <summary>
    /// Initializes a new instance of the WeatherForecastController
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="mediator">MediatR mediator instance</param>
    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Gets weather forecast data
    /// </summary>
    /// <param name="days">Number of days to forecast (default: 5)</param>
    /// <returns>Weather forecast data in JSON or XML format based on Accept header</returns>
    /// <response code="200">Returns the weather forecast</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet]
    [ProducesResponseType(typeof(WeatherForecast[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] [Range(1, 30)] int days = 5)
    {
        _logger.LogInformation("Getting weather forecast for {Days} days", days);
        
        var request = new GetWeatherForecastQuery { Days = days };
        var forecast = await _mediator.Send(request);
        var forecastArray = forecast.ToArray();
        
        // Check if client wants XML
        var acceptHeader = Request.Headers.Accept.ToString();
        if (acceptHeader.Contains("application/xml"))
        {
            var xmlResponse = new WeatherForecastCollection { Items = forecastArray };
            return Ok(xmlResponse);
        }
        
        return Ok(forecastArray);
    }

    /// <summary>
    /// Gets weather forecast for a specific date
    /// </summary>
    /// <param name="date">The date to get forecast for</param>
    /// <returns>Weather forecast for the specified date</returns>
    /// <response code="200">Returns the weather forecast for the date</response>
    /// <response code="400">If the date is invalid</response>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(WeatherForecast), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDate(DateOnly date)
    {
        var request = new GetWeatherForecastByDateQuery { Date = date };
        var forecast = await _mediator.Send(request);
        
        return Ok(forecast);
    }
}
