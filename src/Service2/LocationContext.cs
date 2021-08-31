using Genetec.Service2.Models;
using Microsoft.EntityFrameworkCore;

namespace Genetec.Service2;

/// <summary>
/// DBContext of locations.
/// </summary>
public class LocationContext : DbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationContext"/> class.
    /// </summary>
    /// <param name="options">Options.</param>
    public LocationContext(DbContextOptions<LocationContext> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the dbset.
    /// </summary>
    public DbSet<Location> Locations { get; set; }
}
