using Genetec.Service2.Models;

namespace Genetec.Service2;

internal static class DbInitializer
{
    public static void Initialize(LocationContext context)
    {
        context.Database.EnsureCreated();

        if (context.Locations.Any())
        {
            return;   // DB has been seeded
        }

        var locations = new Location[]
        {
        new Location("1", "Montreal"),
        new Location("2", "Laval"),
        new Location("3", "Brossard"),
        };

        foreach (var l in locations)
        {
            context.Locations.Add(l);
        }

        context.SaveChanges();
    }
}
