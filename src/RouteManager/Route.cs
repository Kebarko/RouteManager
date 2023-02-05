using System;
using System.IO;

namespace KE.MSTS.RouteManager;

internal class Route
{
    public string Name { get; set; } = null!;

    public string Global { get; set; } = null!;

    public string Sound { get; set; } = null!;
    
    public string GetRouteDir(Place place)
    {
        switch (place)
        {
            case Place.TrainSim:
                return Path.Combine(Configuration.Instance.TrainSimPath!, "ROUTES", Name);
            case Place.ExtStorage:
                return Path.Combine(Configuration.Instance.ExtStoragePath!, "ROUTES", Name);
            default:
                throw new ArgumentOutOfRangeException(nameof(place), place, null);
        }
    }

    public string GetGlobalDir(Place place)
    {
        switch (place)
        {
            case Place.TrainSim:
                return Path.Combine(Configuration.Instance.TrainSimPath!, "GLOBAL");
            case Place.ExtStorage:
                return Path.Combine(Configuration.Instance.ExtStoragePath!, Global);
            default:
                throw new ArgumentOutOfRangeException(nameof(place), place, null);
        }
    }

    public string GetSoundDir(Place place)
    {
        switch (place)
        {
            case Place.TrainSim:
                return Path.Combine(Configuration.Instance.TrainSimPath!, "SOUND");
            case Place.ExtStorage:
                return Path.Combine(Configuration.Instance.ExtStoragePath!, Sound);
            default:
                throw new ArgumentOutOfRangeException(nameof(place), place, null);
        }
    }
}
