using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace KE.MSTS.RouteManager;

/// <summary>
/// Represents a component to handle routes.
/// </summary>
internal class Mover
{
    private readonly ICollection<MovableRoute> routes;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mover"/> class.
    /// </summary>
    public Mover()
    {
        routes = new List<MovableRoute>();
    }

    /// <summary>
    /// Gets the list of routes which are located in the Train Simulator.
    /// </summary>
    public IList<MovableRoute> GetActiveRoutes()
    {
        return routes.Where(r => r.CurrentPlace == Place.TrainSim).OrderBy(r => r.Name).ToList();
    }

    /// <summary>
    /// Gets the list of routes which are located in the external storage.
    /// </summary>
    /// <returns></returns>
    public IList<MovableRoute> GetInactiveRoutes()
    {
        return routes.Where(r => r.CurrentPlace == Place.ExtStorage).OrderBy(r => r.Name).ToList();
    }

    /// <summary>
    /// Discovers the current states of routes.
    /// </summary>
    public void DiscoverCurrentState()
    {
        routes.Clear();
        foreach (Route route in Configuration.Instance.Routes)
        {
            if (Directory.Exists(route.GetRoutePath(Place.TrainSim)))
            {
                // The route is in the Train Simulator.
                routes.Add(new MovableRoute(route, Place.TrainSim));
            }
            else if (Directory.Exists(route.GetRoutePath(Place.ExtStorage)))
            {
                // The route is in the external storage.
                routes.Add(new MovableRoute(route, Place.ExtStorage));
            }
            else
            {
                // The route not found.
                throw new ApplicationException($"The route '{route.Name}' not found!");
            }
        }

        // Get the name of the currently used global in Train Simulator
        string? global = routes.FirstOrDefault(r => r.CurrentPlace == Place.TrainSim)?.Global;

        // Get the name of the currently used sound in Train Simulator
        string? sound = routes.FirstOrDefault(r => r.CurrentPlace == Place.TrainSim)?.Sound;

        foreach (MovableRoute route in routes)
        {
            if (route.CurrentPlace == Place.TrainSim)
            {
                // The route is in the Train Simulator

                if (!Directory.Exists(route.GetGlobalPath(Place.TrainSim)))
                    throw new ApplicationException($"Global of the '{route.Name}' route not found!");

                if (!Directory.Exists(route.GetSoundPath(Place.TrainSim)))
                    throw new ApplicationException($"Sound of the '{route.Name}' route not found!");

                if (Directory.Exists(route.GetGlobalPath(Place.ExtStorage)))
                    throw new ApplicationException($"The route '{route.Name}' is in the Train Simulator but its global '{route.Global}' is in the external storage!");

                if (Directory.Exists(route.GetSoundPath(Place.ExtStorage)))
                    throw new ApplicationException($"The route '{route.Name}' is in the Train Simulator but its sound '{route.Sound}' is in the external storage!");

                if (routes.Any(r => r.CurrentPlace == Place.TrainSim && r.Global != route.Global))
                    throw new ApplicationException($"The route '{route.Name}' cannot be in the Train Simulator because its global is different from global of other routes!");

                if (routes.Any(r => r.CurrentPlace == Place.TrainSim && r.Sound != route.Sound))
                    throw new ApplicationException($"The route '{route.Name}' cannot be in the Train Simulator because its sound is different from sound of other routes!");

                route.CurrentColor = Colors.Green;
            }
            else if (route.CurrentPlace == Place.ExtStorage)
            {
                // The route is in the external storage

                if (!Directory.Exists(route.GetGlobalPath(Place.ExtStorage)) && !routes.Any(r => r.CurrentPlace == Place.TrainSim && r.Global == route.Global))
                    throw new ApplicationException($"Global of the '{route.Name}' route not found!");

                if (!Directory.Exists(route.GetSoundPath(Place.ExtStorage)) && !routes.Any(r => r.CurrentPlace == Place.TrainSim && r.Sound == route.Sound))
                    throw new ApplicationException($"Sound of the '{route.Name}' route not found!");

                if (global != null && sound != null)
                {
                    if (route.Global != global && route.Sound != sound)
                    {
                        // Incompatible route
                        route.CurrentColor = Colors.Red;
                    }
                    else if (route.Global != global || route.Sound != sound)
                    {
                        // Partially compatible route
                        route.CurrentColor = Colors.DodgerBlue;
                    }
                    else
                    {
                        // Compatible route
                        route.CurrentColor = Colors.Green;
                    }
                }
                else
                {
                    route.CurrentColor = Colors.Black;
                }
            }
        }
    }

    /// <summary>
    /// Moves the specified route to the opposite side.
    /// </summary>
    /// <param name="route">The route to move.</param>
    public void Move(MovableRoute route)
    {
        if (route.CurrentPlace == Place.TrainSim)
        {
            // The route is in the Train Simulator

            MoveDirectory(route.GetRoutePath(Place.TrainSim), route.GetRoutePath(Place.ExtStorage));

            if (routes.Count(r => r.CurrentPlace == Place.TrainSim) == 1)
            {
                MoveDirectory(route.GetGlobalPath(Place.TrainSim), route.GetGlobalPath(Place.ExtStorage));
                MoveDirectory(route.GetSoundPath(Place.TrainSim), route.GetSoundPath(Place.ExtStorage));
            }

            route.CurrentPlace = Place.ExtStorage;
        }
        else if (route.CurrentPlace == Place.ExtStorage)
        {
            // The route is in the external storage

            IList<MovableRoute> conflictingRoutes = routes.Where(r => r.CurrentPlace == Place.TrainSim && (r.Global != route.Global || r.Sound != route.Sound)).ToList();

            if (conflictingRoutes.Count > 0)
            {
                foreach (MovableRoute conflictingRoute in conflictingRoutes)
                {
                    MoveDirectory(conflictingRoute.GetRoutePath(Place.TrainSim), conflictingRoute.GetRoutePath(Place.ExtStorage));
                }

                MovableRoute oneConflictingRoute = conflictingRoutes.DistinctBy(r => r.Global + r.Sound).Single();

                if (oneConflictingRoute.Global != route.Global)
                {
                    MoveDirectory(oneConflictingRoute.GetGlobalPath(Place.TrainSim), oneConflictingRoute.GetGlobalPath(Place.ExtStorage));
                }

                if (oneConflictingRoute.Sound != route.Sound)
                {
                    MoveDirectory(oneConflictingRoute.GetSoundPath(Place.TrainSim), oneConflictingRoute.GetSoundPath(Place.ExtStorage));
                }
            }

            if (Directory.Exists(route.GetGlobalPath(Place.ExtStorage)))
            {
                MoveDirectory(route.GetGlobalPath(Place.ExtStorage), route.GetGlobalPath(Place.TrainSim));
            }

            if (Directory.Exists(route.GetSoundPath(Place.ExtStorage)))
            {
                MoveDirectory(route.GetSoundPath(Place.ExtStorage), route.GetSoundPath(Place.TrainSim));
            }

            MoveDirectory(route.GetRoutePath(Place.ExtStorage), route.GetRoutePath(Place.TrainSim));

            route.CurrentPlace = Place.TrainSim;
        }
    }

    private static void MoveDirectory(string sourceDirName, string destDirName)
    {
        Directory.Move(sourceDirName, destDirName);

        Log.Information($"Directory '{sourceDirName}' moved to '{destDirName}'.");
    }
}
