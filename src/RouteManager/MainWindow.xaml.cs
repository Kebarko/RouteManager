using System;
using System.Windows;

namespace KE.MSTS.RouteManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private Mover mover = null!;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            mover = new Mover();
            mover.DiscoverCurrentState();
            ActiveRoutes.ItemsSource = mover.GetActiveRouteNames();
            InactiveRoutes.ItemsSource = mover.GetInactiveRouteNames();
        }
        catch (Exception ex)
        {
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
            mover.DiscoverCurrentState();
            ActiveRoutes.ItemsSource = mover.GetActiveRouteNames();
            InactiveRoutes.ItemsSource = mover.GetInactiveRouteNames();
        }
        catch (Exception ex)
        {
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
            mover.DiscoverCurrentState();
            ActiveRoutes.ItemsSource = mover.GetActiveRouteNames();
            InactiveRoutes.ItemsSource = mover.GetInactiveRouteNames();
        }
        catch (Exception ex)
        {
            MoveToExtStorage.IsEnabled = MoveToTrainSim.IsEnabled = false;
            MessageBox.Show(this, ex.InnerException?.Message ?? ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}