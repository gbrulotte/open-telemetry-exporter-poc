using Genetec.Service1.Models;

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
        using var client = _clientFactory.CreateClient("Service2");
        var locations = await client.GetFromJsonAsync<IReadOnlyCollection<Location>>("/api/locations");

        var counter = Startup.Meter.CreateCounter<int>("Requests");
        counter.Add(1, KeyValuePair.Create<string, object?>("request", "read"));

        _logger.LogDebug("Inside weather forecast controller.");

        using (var activity = Startup.ActivitySource.StartActivity("Computting weather forecasts"))
        {
            // Simulating CPU intensive load.
            await Task.Delay(200);
        }

        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Location = locations?.ElementAt(rng.Next(0, locations?.Count ?? 0)).Name,
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)],
        })
        .ToList();
    }
}
