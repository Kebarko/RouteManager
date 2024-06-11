using KE.MSTS.RouteManager.Exceptions;
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
    private readonly List<MovableRoute> routes;

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
    public IList<MovableRoute> GetInactiveRoutes()
    {
        return routes.Where(r => r.CurrentPlace == Place.ExtStorage).OrderBy(r => r.Name).ToList();
    }

    /// <summary>
    /// Discovers the current state of routes.
    /// </summary>
    public void DiscoverCurrentState()
    {
        // Create instances of movable routes according to configuration
        routes.Clear();
        foreach (Route route in Configuration.Instance.Routes)
        {
            // Check if the route is in Train Simulator
            if (Directory.Exists(route.GetRoutePath(Place.TrainSim)))
            {
                routes.Add(new MovableRoute(route, Place.TrainSim));
            }
            // Check if the route is in external storage
            else if (Directory.Exists(route.GetRoutePath(Place.ExtStorage)))
            {
                routes.Add(new MovableRoute(route, Place.ExtStorage));
            }
            else
            {
                // The route was not found
                throw new InvalidStateException($"The route '{route.Name}' was not found!");
            }
        }

        bool allRoutesInExtStorage = routes.All(r => r.CurrentPlace == Place.ExtStorage);
        List<MovableRoute> routesInTrainSim = routes.Where(r => r.CurrentPlace == Place.TrainSim).ToList();

        // Iterate over each route
        foreach (MovableRoute route in routes)
        {
            if (route.CurrentPlace == Place.TrainSim)
            {
                // Check the presence of each folder belonging to the route
                foreach (KeyValuePair<string, string> folder in route.Folders)
                {
                    // Check if the specified folder exists in Train Simulator
                    if (!Directory.Exists(route.GetFolderPath(folder.Key, Place.TrainSim)))
                        throw new InvalidStateException($"{folder.Key} of the '{route.Name}' route not found!");

                    // Check if the specified folder does not exist in external storage
                    if (Directory.Exists(route.GetFolderPath(folder.Value, Place.ExtStorage)))
                        throw new InvalidStateException($"The route '{route.Name}' is in the Train Simulator but its '{folder.Key}' is in the external storage!");

                    // Check if there is no other route in Train Simulator that has a different folder (e.g. different Global or Sound)
                    if (routes.Any(r => r != route && r.CurrentPlace == Place.TrainSim && r.Folders.ContainsKey(folder.Key) && !r.Folders[folder.Key].Equals(folder.Value, StringComparison.OrdinalIgnoreCase)))
                        throw new InvalidStateException($"The route '{route.Name}' cannot be in the Train Simulator because its '{folder.Key}' is different from '{folder.Key}' of other routes!");
                }

                route.CurrentColor = Colors.Green;
            }
            else if (route.CurrentPlace == Place.ExtStorage)
            {
                // Check the presence of each folder belonging to the route
                foreach (KeyValuePair<string, string> folder in route.Folders)
                {
                    // Check if the specified folder exists either in external storage or in Train Simulator
                    if (!Directory.Exists(route.GetFolderPath(folder.Value, Place.ExtStorage)) && !routes.Any(r => r.CurrentPlace == Place.TrainSim && r.Folders.ContainsKey(folder.Key) && r.Folders[folder.Key].Equals(folder.Value, StringComparison.OrdinalIgnoreCase)))
                        throw new InvalidStateException($"{folder.Key} of the '{route.Name}' route not found!");
                }

                // Determine compatibility with routes in Train Simulator
                if (allRoutesInExtStorage)
                {
                    route.CurrentColor = Colors.Black;
                }
                else
                {
                    if (routesInTrainSim.All(route.IsCompatible))
                    {
                        // Compatible route
                        route.CurrentColor = Colors.Green;
                    }
                    else if (routesInTrainSim.Any(route.IsPartiallyCompatible))
                    {
                        // Partially compatible route
                        route.CurrentColor = Colors.DodgerBlue;
                    }
                    else
                    {
                        // Incompatible route
                        route.CurrentColor = Colors.Red;
                    }
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
        Log.Information($"Start of the moving route '{route.Name}'.");

        if (route.CurrentPlace == Place.TrainSim)
        {
            bool isOnlyRoute = routes.Count(r => r.CurrentPlace == Place.TrainSim) == 1;
            List<MovableRoute> routesInTrainSim = routes.Where(r => r.CurrentPlace == Place.TrainSim).ToList();

            // Move route directory to external storage
            MoveDirectory(route.GetRoutePath(Place.TrainSim), route.GetRoutePath(Place.ExtStorage));

            // Move all unused directories to external storage, as we would no longer be able to identify them
            foreach (KeyValuePair<string, string> folder in route.Folders)
            {
                if (isOnlyRoute || !routesInTrainSim.Any(r => r != route && r.Folders.ContainsKey(folder.Key)))
                {
                    MoveDirectory(route.GetFolderPath(folder.Key, Place.TrainSim), route.GetFolderPath(folder.Value, Place.ExtStorage));
                }
            }

            route.CurrentPlace = Place.ExtStorage;
        }
        else if (route.CurrentPlace == Place.ExtStorage)
        {
            // Check if there are incompatible routes
            List<MovableRoute> incompatibleRoutes = routes.Where(r => r.CurrentPlace == Place.TrainSim && !route.IsCompatible(r)).ToList();
            if (incompatibleRoutes.Count > 0)
            {
                // Move all incompatible routes and their folders to external storage
                foreach (MovableRoute incompatibleRoute in incompatibleRoutes)
                {
                    MoveDirectory(incompatibleRoute.GetRoutePath(Place.TrainSim), incompatibleRoute.GetRoutePath(Place.ExtStorage));

                    foreach (KeyValuePair<string, string> folder in incompatibleRoute.Folders)
                    {
                        if (Directory.Exists(route.GetFolderPath(folder.Key, Place.TrainSim)))
                        {
                            MoveDirectory(route.GetFolderPath(folder.Key, Place.TrainSim), route.GetFolderPath(folder.Value, Place.ExtStorage));
                        }
                    }
                }
            }

            // Move route directory to Train Simulator
            MoveDirectory(route.GetRoutePath(Place.ExtStorage), route.GetRoutePath(Place.TrainSim));

            // Move all necessary directories to Train Simulator (if they do not already present)
            foreach (KeyValuePair<string, string> folder in route.Folders)
            {
                if (Directory.Exists(route.GetFolderPath(folder.Value, Place.ExtStorage)))
                {
                    MoveDirectory(route.GetFolderPath(folder.Value, Place.ExtStorage), route.GetFolderPath(folder.Key, Place.TrainSim));
                }
            }

            route.CurrentPlace = Place.TrainSim;
        }

        Log.Information($"End of the moving route '{route.Name}'.");
    }

    /// <summary>
    /// Moves directory to a new location and write a log event.
    /// </summary>
    private static void MoveDirectory(string sourceDirName, string destDirName)
    {
        Directory.Move(sourceDirName, destDirName);

        Log.Information($"Directory '{sourceDirName}' moved to '{destDirName}'.");
    }
}
