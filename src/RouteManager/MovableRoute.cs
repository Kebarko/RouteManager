using System.Windows.Media;

namespace KE.MSTS.RouteManager;

internal class MovableRoute : Route
{
    public Place CurrentPlace { get; set; }

    public Color CurrentColor { get; set; }

    public MovableRoute()
    {
    }

    public MovableRoute(Route route, Place currentPlace)
    {
        Name = route.Name;
        Global = route.Global;
        Sound = route.Sound;

        CurrentPlace = currentPlace;
    }
}
