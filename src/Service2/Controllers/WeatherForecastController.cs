namespace Genetec.Enrollment.Management.Api;

/// <summary>
/// Weather forecast controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    };

    private readonly ILogger<WeatherForecastController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherForecastController"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger{TCategoryName}"/> instance.</param>
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets all weather forecast.
    /// </summary>
    /// <returns>A list of weather forecast.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<WeatherForecast> Get()
    {
        var counter = Startup.Meter.CreateCounter<int>("Requests");
        counter.Add(1, KeyValuePair.Create<string, object?>("request", "read"));

        _logger.LogDebug("Inside weather forecast controller.");

        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)],
        })
        .ToList();
    }
}
