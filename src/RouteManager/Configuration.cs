using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KE.MSTS.RouteManager;

internal sealed class Configuration
{
    public static Configuration Instance { get; } = new();

    public string TrainSimPath { get; set; }

    public string ExtStoragePath { get; set; }

    public ICollection<Route> Routes { get; set; }

    static Configuration()
    {
    }

    private Configuration()
    {
        using (Stream sr = new FileStream("configuration.json", FileMode.Open, FileAccess.Read, FileShare.None))
        {
            JsonDocument jDoc = JsonDocument.Parse(sr);

            if (jDoc == null)
                throw new ApplicationException("Configuration not found!");

            jDoc.RootElement.GetProperty(nameof(TrainSimPath));

            TrainSimPath = jDoc.RootElement.GetProperty(nameof(TrainSimPath)).GetString();
            ExtStoragePath = jDoc.RootElement.GetProperty(nameof(ExtStoragePath)).GetString();
            Routes = jDoc.RootElement.GetProperty(nameof(Routes)).Deserialize<List<Route>>() ?? new List<Route>();
        }

        if (!Path.Exists(TrainSimPath))
            throw new ApplicationException(@$"{TrainSimPath}{Environment.NewLine}The specified path does not exist!");

        if (!Path.Exists(ExtStoragePath))
            throw new ApplicationException(@$"{ExtStoragePath}{Environment.NewLine}The specified path does not exist!");

        if (Path.GetPathRoot(TrainSimPath) != Path.GetPathRoot(ExtStoragePath))
            throw new ApplicationException("Train Simulator and external storage must be on the same volume!");

        if (TrainSimPath.StartsWith(ExtStoragePath, StringComparison.InvariantCultureIgnoreCase) ||
            ExtStoragePath.StartsWith(TrainSimPath, StringComparison.InvariantCultureIgnoreCase))
            throw new ApplicationException("Train Simulator and external storage must be in different folders!");

        if (Routes.Count == 0)
            throw new ApplicationException("Routes not found!");

        foreach (Route confRoute in Routes)
        {
            if (string.IsNullOrEmpty(confRoute.Name))
                throw new ApplicationException("Route name is undefined!");

            if (string.IsNullOrEmpty(confRoute.Global))
                throw new ApplicationException($"Global of the '{confRoute.Name}' route is undefined!");

            if (string.IsNullOrEmpty(confRoute.Sound))
                throw new ApplicationException($"Sound of the '{confRoute.Name}' route is undefined!");
        }
    }
}
