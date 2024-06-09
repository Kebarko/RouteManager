using KE.MSTS.RouteManager.Exceptions;
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
            JsonDocument jDoc = JsonDocument.Parse(sr) ?? throw new BadConfigurationException("Configuration not found!");

            string? trainSimPath = jDoc.RootElement.GetProperty(nameof(TrainSimPath)).GetString();
            string? extStoragePath = jDoc.RootElement.GetProperty(nameof(ExtStoragePath)).GetString();

            if (!Path.Exists(trainSimPath))
                throw new BadConfigurationException(@$"{trainSimPath}{Environment.NewLine}The specified path does not exist!");

            if (!Path.Exists(extStoragePath))
                throw new BadConfigurationException(@$"{extStoragePath}{Environment.NewLine}The specified path does not exist!");

            if (Path.GetPathRoot(trainSimPath) != Path.GetPathRoot(extStoragePath))
                throw new BadConfigurationException("Train Simulator and external storage must be on the same volume!");

            if (trainSimPath.StartsWith(extStoragePath, StringComparison.OrdinalIgnoreCase) ||
                extStoragePath.StartsWith(trainSimPath, StringComparison.OrdinalIgnoreCase))
                throw new BadConfigurationException("Train Simulator and external storage must be in different folders!");

            TrainSimPath = trainSimPath;
            ExtStoragePath = extStoragePath;

            JsonElement routes = jDoc.RootElement.GetProperty(nameof(Routes));
            foreach (JsonElement route in routes.EnumerateArray())
            {
                string? name = null;
                var folders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (JsonProperty property in route.EnumerateObject())
                {
                    string? value = property.Value.GetString();
                    if (!string.IsNullOrEmpty(property.Name) && !string.IsNullOrEmpty(value))
                    {
                        if (property.Name == "Name")
                        {
                            name = value;
                        }
                        else
                        {
                            folders.Add(property.Name, value);
                        }
                    }
                }

                if (string.IsNullOrEmpty(name))
                    throw new BadConfigurationException("Route name is undefined!");

                if (!folders.ContainsKey(Route.Global))
                    throw new BadConfigurationException($"Global of the '{name}' route is undefined!");

                if (!folders.ContainsKey(Route.Global))
                    throw new BadConfigurationException($"Sound of the '{name}' route is undefined!");

                Routes.Add(new Route(name, folders));
            }

            if (Routes.Count == 0)
                throw new BadConfigurationException("Routes not found!");
        }
    }
}
