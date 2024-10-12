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

        if (ViewModel?.IsAuth == true)
        {
            return mainWindow;
        }

        mainWindow.ViewModel!.Router.Navigate.Execute(new LoginPageViewModel(mainWindow.ViewModel!, mainWindow.ViewModel!.OnClosed));
        mainWindow.ViewModel.Manager
            .CreateMessage(true, "#D03E3E",
                ViewModel!.LocalizationService.GetString(ResourceKeysDictionary.Error),
                ViewModel!.LocalizationService.GetString(ResourceKeysDictionary.InvalidSession))
            .Dismiss()
            .WithDelay(TimeSpan.FromSeconds(3))
            .Queue();
        return mainWindow;

    }
}
