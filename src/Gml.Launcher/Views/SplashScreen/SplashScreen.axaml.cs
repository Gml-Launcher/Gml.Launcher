using System;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using GamerVII.Notification.Avalonia;
using Gml.Launcher.Assets;
using Gml.Launcher.ViewModels;
using Gml.Launcher.ViewModels.Pages;

namespace Gml.Launcher.Views.SplashScreen;

public partial class SplashScreen : ReactiveWindow<SplashScreenViewModel>
{
    public SplashScreen()
    {
        InitializeComponent();
    }

    public MainWindow GetMainWindow()
    {
        var mainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel()
        };

        return mainWindow;
    }
}
