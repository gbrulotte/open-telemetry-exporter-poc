using System.ComponentModel.DataAnnotations;

namespace Genetec.Enrollment.Management.Api.Models;

/// <summary>
/// Representation of a weather forecast.
/// </summary>
public class WeatherForecast
{
    /// <summary>
    /// Gets the location.
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// Gets the weather forecast date.
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Gets the weather forecast temperature in Celsius.
    /// </summary>
    public int TemperatureC { get; init; }

    /// <summary>
    /// Gets the weather forecast temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Gets the weather forecast summary.
    /// </summary>
    [Required]
    public string Summary { get; init; } = string.Empty;
}
