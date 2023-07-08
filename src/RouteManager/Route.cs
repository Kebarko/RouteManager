using System;
using System.IO;

namespace KE.MSTS.RouteManager;

/// <summary>
/// Represents a Train Simulator route.
/// </summary>
internal class Route
{
    /// <summary>
    /// Gets or sets the name of the route.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the name of the route's global.
    /// </summary>
    public string? Global { get; set; }

    /// <summary>
    /// Gets or sets the name of the route's sound.
    /// </summary>
    public string? Sound { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Route"/> class.
    /// </summary>
    public Route()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Route"/> class with values of the specified route.
    /// </summary>
    /// <param name="route">The route to copy from.</param>
    public Route(Route route)
        : this(route.Name, route.Global, route.Sound)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Route"/> class with the specified name, global name and sound name.
    /// </summary>
    /// <param name="name">The route name.</param>
    /// <param name="global">The global name.</param>
    /// <param name="sound">The sound name.</param>
    public Route(string? name, string? global, string? sound)
    {
        Name = name;
        Global = global;
        Sound = sound;
    }

    /// <summary>
    /// Gets the full path of the route on the specified place.
    /// </summary>
    public string GetRoutePath(Place place)
    {
        return place switch
        {
            Place.TrainSim => Path.Combine(Configuration.Instance.TrainSimPath, "ROUTES", Name!),
            Place.ExtStorage => Path.Combine(Configuration.Instance.ExtStoragePath, "ROUTES", Name!),
            _ => throw new ArgumentOutOfRangeException(nameof(place), place, null),
        };
    }

    /// <summary>
    /// Gets the full path of the route's global on the specified place.
    /// </summary>
    public string GetGlobalPath(Place place)
    {
        return place switch
        {
            Place.TrainSim => Path.Combine(Configuration.Instance.TrainSimPath, "GLOBAL"),
            Place.ExtStorage => Path.Combine(Configuration.Instance.ExtStoragePath, Global!),
            _ => throw new ArgumentOutOfRangeException(nameof(place), place, null),
        };
    }

    /// <summary>
    /// Gets the full path of the route's sound on the specified place.
    /// </summary>
    public string GetSoundPath(Place place)
    {
        return place switch
        {
            Place.TrainSim => Path.Combine(Configuration.Instance.TrainSimPath, "SOUND"),
            Place.ExtStorage => Path.Combine(Configuration.Instance.ExtStoragePath, Sound!),
            _ => throw new ArgumentOutOfRangeException(nameof(place), place, null),
        };
    }
}
