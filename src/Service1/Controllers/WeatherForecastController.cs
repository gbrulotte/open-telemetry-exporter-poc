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
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherForecastController"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger{TCategoryName}"/> instance.</param>
    /// <param name="clientFactory">HTTP client factory.</param>
    public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    /// <summary>
    /// Gets all weather forecast.
    /// </summary>
    /// <returns>A list of weather forecast.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IEnumerable<WeatherForecast>> GetAsync()
    {
        IReadOnlyCollection<WeatherForecast>? otherForecasts = null;

        using (var activity = Startup.ActivitySource.StartActivity("Getting weather forecasts"))
        {
            using var client = _clientFactory.CreateClient("Service2");
            otherForecasts = await client.GetFromJsonAsync<IReadOnlyCollection<WeatherForecast>>("/api/WeatherForecast");
        }

        var counter = Startup.Meter.CreateCounter<int>("Requests");
        counter.Add(1, KeyValuePair.Create<string, object?>("request", "read"));

        _logger.LogDebug("Inside weather forecast controller.");

        var rng = new Random();
        var foreacasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)],
        })
        .ToList();

        foreacasts.AddRange(otherForecasts ?? Array.Empty<WeatherForecast>());

        return foreacasts;
    }
}
