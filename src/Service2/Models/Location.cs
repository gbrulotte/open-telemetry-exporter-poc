namespace Genetec.Service2.Models;

/// <summary>
/// Represent a location.
/// </summary>
public class Location
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Location"/> class.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="name">Name.</param>
    public Location(string id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Gets or sets the location id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    public string Name { get; set; }
}
