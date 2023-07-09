using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KE.MSTS.RouteManager;

/// <summary>
/// Represents a configuration.
/// </summary>
internal sealed class Configuration
{
    /// <summary>
    /// Gets the configuration instance
    /// </summary>
    public static Configuration Instance { get; } = new();

    /// <summary>
    /// Gets the Train Simulator full path.
    /// </summary>
    public string TrainSimPath { get; private set; }

    /// <summary>
    /// Gets the external storage full path.
    /// </summary>
    public string ExtStoragePath { get; private set; }

    /// <summary>
    /// Gets the collection of routes.
    /// </summary>
    public ICollection<Route> Routes { get; } = new List<Route>();

    static Configuration()
    {
    }

    private Configuration()
    {
        using (Stream sr = new FileStream("configuration.json", FileMode.Open, FileAccess.Read, FileShare.None))
        {
            JsonDocument jDoc = JsonDocument.Parse(sr) ?? throw new ApplicationException("Configuration not found!");

            string? trainSimPath = jDoc.RootElement.GetProperty(nameof(TrainSimPath)).GetString();
            string? extStoragePath = jDoc.RootElement.GetProperty(nameof(ExtStoragePath)).GetString();

            if (!Path.Exists(trainSimPath))
                throw new ApplicationException(@$"{trainSimPath}{Environment.NewLine}The specified path does not exist!");

            if (!Path.Exists(extStoragePath))
                throw new ApplicationException(@$"{extStoragePath}{Environment.NewLine}The specified path does not exist!");

            if (Path.GetPathRoot(trainSimPath) != Path.GetPathRoot(extStoragePath))
                throw new ApplicationException("Train Simulator and external storage must be on the same volume!");

            if (trainSimPath.StartsWith(extStoragePath, StringComparison.InvariantCultureIgnoreCase) ||
                extStoragePath.StartsWith(trainSimPath, StringComparison.InvariantCultureIgnoreCase))
                throw new ApplicationException("Train Simulator and external storage must be in different folders!");

            TrainSimPath = trainSimPath;
            ExtStoragePath = extStoragePath;

            JsonElement routes = jDoc.RootElement.GetProperty(nameof(Routes));
            foreach (JsonElement route in routes.EnumerateArray())
            {
                string? name = route.GetProperty(nameof(Route.Name)).GetString();
                string? global = route.GetProperty(nameof(Route.Global)).GetString();
                string? sound = route.GetProperty(nameof(Route.Sound)).GetString();

                if (string.IsNullOrEmpty(name))
                    throw new ApplicationException("Route name is undefined!");

                if (string.IsNullOrEmpty(global))
                    throw new ApplicationException($"Global of the '{name}' route is undefined!");

                if (string.IsNullOrEmpty(sound))
                    throw new ApplicationException($"Sound of the '{name}' route is undefined!");

                Routes.Add(new Route(name, global, sound));
            }

            if (Routes.Count == 0)
                throw new ApplicationException("Routes not found!");
        }
    }
}
