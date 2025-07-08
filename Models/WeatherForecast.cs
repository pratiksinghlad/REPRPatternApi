using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace REPRPatternApi.Models;

/// <summary>
/// Weather forecast data transfer object
/// </summary>
[XmlRoot("WeatherForecast")]
public class WeatherForecast
{
    /// <summary>
    /// Gets or sets the date of the forecast
    /// </summary>
    [XmlElement("Date")]
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the temperature in Celsius
    /// </summary>
    [XmlElement("TemperatureC")]
    public int TemperatureC { get; set; }

    /// <summary>
    /// Gets or sets the temperature in Fahrenheit
    /// </summary>
    [XmlElement("TemperatureF")]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Gets or sets the weather summary
    /// </summary>
    [XmlElement("Summary")]
    public string Summary { get; set; } = string.Empty;
}

/// <summary>
/// Weather forecast collection for XML serialization
/// </summary>
[XmlRoot("WeatherForecasts")]
public class WeatherForecastCollection
{
    /// <summary>
    /// Gets or sets the weather forecast items
    /// </summary>
    [XmlElement("WeatherForecast")]
    public WeatherForecast[] Items { get; set; } = Array.Empty<WeatherForecast>();
}