using Genetec.Service2.Models;

namespace Genetec.Service2.Controllers;

/// <summary>
/// Weather forecast controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILogger<LocationsController> _logger;
    private readonly LocationContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocationsController"/> class.
    /// </summary>
    /// <param name="logger"><see cref="ILogger{TCategoryName}"/> instance.</param>
    /// <param name="context">DbContext instance.</param>
    public LocationsController(ILogger<LocationsController> logger, LocationContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Gets all weather forecast.
    /// </summary>
    /// <returns>A list of weather forecast.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<Location> Get()
    {
        _logger.LogDebug("Inside locations controller.");

        return _context.Locations.ToList();
    }
}
