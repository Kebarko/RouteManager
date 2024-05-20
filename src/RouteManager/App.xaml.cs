using Serilog;
using System.Windows;

namespace KE.MSTS.RouteManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                path: "logs/RouteManager.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: false,
                retainedFileCountLimit: 3,
                shared: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        Log.CloseAndFlush();
    }
}
