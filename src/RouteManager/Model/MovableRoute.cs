namespace KE.MSTS.RouteManager.Model;

/// <summary>
/// Represents a Train Simulator route that can be moved between Train Simulator and external storage.
/// </summary>
internal class MovableRoute : Route
{
    /// <summary>
    /// Gets or sets the current place of the route.
    /// </summary>
    public Place CurrentPlace { get; set; }

    /// <summary>
    /// Gets or sets the current compatibility of the route.
    /// </summary>
    public Compatibility CurrentCompatibility { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MovableRoute"/> class with values of the specified route and current place.
    /// </summary>
    /// <param name="route">The route to create from.</param>
    /// <param name="currentPlace">The current place of the route.</param>
    public MovableRoute(Route route, Place currentPlace)
        : base(route)
    {
        CurrentPlace = currentPlace;
    }
}
