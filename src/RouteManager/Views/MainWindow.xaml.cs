using KE.MSTS.RouteManager.Model;
using Serilog;
using System;
using System.Reflection;
using System.Windows;

namespace KE.MSTS.RouteManager.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly Mover mover = new();

    public MainWindow()
    {
        InitializeComponent();

        Title = $"RouteManager {Assembly.GetExecutingAssembly().GetName().Version?.ToString(2)}";
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            ReloadState();

            TrainSimPath.Text = Configuration.Instance.TrainSimPath;
            ExtStoragePath.Text = Configuration.Instance.ExtStoragePath;
        }
        catch (Exception ex)
        {
            Log.Error(ex.InnerException?.Message ?? ex.Message);

            MoveToExtStorage.IsEnabled = MoveToTrainSim.IsEnabled = false;
            MessageBox.Show(this, ex.InnerException?.Message ?? ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MoveToExtStorage_OnClick(object sender, RoutedEventArgs e)
    {
        if (ActiveRoutes.SelectedIndex == -1)
            return;

        try
        {
            mover.Move((MovableRoute)ActiveRoutes.SelectedValue);

            ReloadState();
        }
        catch (Exception ex)
        {
            Log.Error(ex.InnerException?.Message ?? ex.Message);

            MoveToExtStorage.IsEnabled = MoveToTrainSim.IsEnabled = false;
            MessageBox.Show(this, ex.InnerException?.Message ?? ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MoveToTrainSim_OnClick(object sender, RoutedEventArgs e)
    {
        if (InactiveRoutes.SelectedIndex == -1)
            return;

        try
        {
            mover.Move((MovableRoute)InactiveRoutes.SelectedValue);

            ReloadState();
        }
        catch (Exception ex)
        {
            Log.Error(ex.InnerException?.Message ?? ex.Message);

            MoveToExtStorage.IsEnabled = MoveToTrainSim.IsEnabled = false;
            MessageBox.Show(this, ex.InnerException?.Message ?? ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ReloadState()
    {
        mover.DiscoverCurrentState();
        ActiveRoutes.ItemsSource = mover.GetActiveRoutes();
        InactiveRoutes.ItemsSource = mover.GetInactiveRoutes();
    }
}
