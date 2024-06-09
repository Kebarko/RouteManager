using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace KE.MSTS.RouteManager;

/// <summary>
/// Represents a Train Simulator route.
/// </summary>
[DebuggerDisplay("{Name}")]
internal class Route
{
    /// <summary>
    /// Represents a name of the Global folder.
    /// </summary>
    public const string Global = "Global";

    /// <summary>
    /// Represent a name of the Sound folder.
    /// </summary>
    public const string Sound = "Sound";

    /// <summary>
    /// Represents a name of the Routes folder.
    /// </summary>
    public const string Routes = "Routes";

    /// <summary>
    /// Gets the name of the route.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the dictionary of route folder in the root directory (Global, Sound, etc.).
    /// </summary>
    public IReadOnlyDictionary<string, string> Folders { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Route"/> class with values of the specified route.
    /// </summary>
    /// <param name="route">The route to copy from.</param>
    public Route(Route route)
        : this(route.Name, route.Folders)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Route"/> class with the specified name, global name and sound name.
    /// </summary>
    /// <param name="name">The route name.</param>
    /// <param name="global">The global name.</param>
    /// <param name="sound">The sound name.</param>
    public Route(string name, IEnumerable<KeyValuePair<string, string>> folders)
    {
        Name = name;
        Folders = folders.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the full path of the route on the specified place.
    /// </summary>
    public string GetRoutePath(Place place)
    {
        return place switch
        {
            Place.TrainSim => Path.Combine(Configuration.Instance.TrainSimPath, Routes, Name),
            Place.ExtStorage => Path.Combine(Configuration.Instance.ExtStoragePath, Routes, Name),
            _ => throw new ArgumentOutOfRangeException(nameof(place), place, null),
        };
    }

    /// <summary>
    /// Gets the full path of the specified route folder on the specified place.
    /// </summary>
    public string GetFolderPath(string folderName, Place place)
    {
        return place switch
        {
            Place.TrainSim => Path.Combine(Configuration.Instance.TrainSimPath, folderName),
            Place.ExtStorage => Path.Combine(Configuration.Instance.ExtStoragePath, folderName),
            _ => throw new ArgumentOutOfRangeException(nameof(place), place, null),
        };
    }

    /// <summary>
    /// Determines whether this route is compatible with the specified route.
    /// </summary>
    public bool IsCompatible(Route route)
    {
        foreach (string key in Folders.Keys.Intersect(route.Folders.Keys, StringComparer.OrdinalIgnoreCase))
        {
            if (!Folders[key].Equals(route.Folders[key], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }
}
