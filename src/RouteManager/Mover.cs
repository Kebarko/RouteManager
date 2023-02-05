using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace KE.MSTS.RouteManager;

internal class Mover
{
    private readonly ICollection<MovableRoute> routes;

    public Mover()
    {
        routes = new List<MovableRoute>();
    }

    public IList<MovableRoute> GetActiveRouteNames()
    {
        return routes.Where(r => r.CurrentPlace == Place.TrainSim).OrderBy(r => r.Name).ToList();
    }

    public IList<MovableRoute> GetInactiveRouteNames()
    {
        return routes.Where(r => r.CurrentPlace == Place.ExtStorage).OrderBy(r => r.Name).ToList();
    }

    public void DiscoverCurrentState()
    {
        routes.Clear();
        foreach (Route route in Configuration.Instance.Routes)
        {
            if (Directory.Exists(route.GetRouteDir(Place.TrainSim)))
            {
                // The route is in the Train Simulator
                routes.Add(new MovableRoute(route, Place.TrainSim));
            }
            else if (Directory.Exists(route.GetRouteDir(Place.ExtStorage)))
            {
                // The route is in the external storage
                routes.Add(new MovableRoute(route, Place.ExtStorage));
            }
            else
            {
                throw new ApplicationException($"The route '{route.Name}' not found!");
            }
        }
        
        string global = GetTrainSimGlobal();
        string sound = GetTrainSimSound();
        
        foreach (MovableRoute route in routes)
        {
            if (route.CurrentPlace == Place.TrainSim)
            {
                // The route is in the Train Simulator

                if (!Directory.Exists(route.GetGlobalDir(Place.TrainSim)))
                    throw new ApplicationException($"Global of the '{route.Name}' route not found!");

                if (!Directory.Exists(route.GetSoundDir(Place.TrainSim)))
                    throw new ApplicationException($"Sound of the '{route.Name}' route not found!");

                if (Directory.Exists(route.GetGlobalDir(Place.ExtStorage)))
                    throw new ApplicationException($"The route '{route.Name}' is in the Train Simulator but its global '{route.Global}' is in the external storage!");

                if (Directory.Exists(route.GetSoundDir(Place.ExtStorage)))
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

                if (!Directory.Exists(route.GetGlobalDir(Place.ExtStorage)) && !routes.Any(r => r.CurrentPlace == Place.TrainSim && r.Global == route.Global))
                    throw new ApplicationException($"Global of the '{route.Name}' route not found!");

                if (!Directory.Exists(route.GetSoundDir(Place.ExtStorage)) && !routes.Any(r => r.CurrentPlace == Place.TrainSim && r.Sound == route.Sound))
                    throw new ApplicationException($"Sound of the '{route.Name}' route not found!");

                if (global != null && sound != null)
                {
                    if (route.Global != global && route.Sound != sound)
                        route.CurrentColor = Colors.Red;
                    else if (route.Global != global || route.Sound != sound)
                        route.CurrentColor = Colors.DodgerBlue;
                    else
                        route.CurrentColor = Colors.Green; 
                }
                else
                {
                    route.CurrentColor = Colors.Black;
                }
            }
        }
    }

    public void Move(MovableRoute route)
    {
        if (route.CurrentPlace == Place.TrainSim)
        {
            // The route is in the Train Simulator

            Directory.Move(route.GetRouteDir(Place.TrainSim), route.GetRouteDir(Place.ExtStorage));

            if (routes.Count(r => r.CurrentPlace == Place.TrainSim) == 1)
            {
                Directory.Move(route.GetGlobalDir(Place.TrainSim), route.GetGlobalDir(Place.ExtStorage));
                Directory.Move(route.GetSoundDir(Place.TrainSim), route.GetSoundDir(Place.ExtStorage));
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
                    Directory.Move(conflictingRoute.GetRouteDir(Place.TrainSim), conflictingRoute.GetRouteDir(Place.ExtStorage));
                }

                MovableRoute oneConflictingRoute = conflictingRoutes.DistinctBy(r => r.Global + r.Sound).Single();

                if (oneConflictingRoute.Global != route.Global)
                {
                    Directory.Move(oneConflictingRoute.GetGlobalDir(Place.TrainSim), oneConflictingRoute.GetGlobalDir(Place.ExtStorage));
                }

                if (oneConflictingRoute.Sound != route.Sound)
                {
                    Directory.Move(oneConflictingRoute.GetSoundDir(Place.TrainSim), oneConflictingRoute.GetSoundDir(Place.ExtStorage));
                }
            }

            if (Directory.Exists(route.GetGlobalDir(Place.ExtStorage)))
            {
                Directory.Move(route.GetGlobalDir(Place.ExtStorage), route.GetGlobalDir(Place.TrainSim));
            }

            if (Directory.Exists(route.GetSoundDir(Place.ExtStorage)))
            {
                Directory.Move(route.GetSoundDir(Place.ExtStorage), route.GetSoundDir(Place.TrainSim));
            }

            Directory.Move(route.GetRouteDir(Place.ExtStorage), route.GetRouteDir(Place.TrainSim));

            route.CurrentPlace = Place.TrainSim;
        }
    }

    private string GetTrainSimGlobal() => routes.Where(r => r.CurrentPlace == Place.TrainSim).Select(r => r.Global).Distinct().SingleOrDefault();

    private string GetTrainSimSound() => routes.Where(r => r.CurrentPlace == Place.TrainSim).Select(r => r.Sound).Distinct().SingleOrDefault();
}
