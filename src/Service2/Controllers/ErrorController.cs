namespace Genetec.Enrollment.Management.Api.Controllers;

/// <summary>
/// Controller handling unexpected errors.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Route("[controller]")]
[ApiController]
public class ErrorController : ControllerBase
{
    /// <summary>
    /// Produces error payload.
    /// </summary>
    /// <returns>Details about the problem.</returns>
    public IActionResult Error() => Problem();
}
