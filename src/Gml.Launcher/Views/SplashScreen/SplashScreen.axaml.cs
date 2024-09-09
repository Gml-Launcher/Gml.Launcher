using Avalonia.Controls;
using Avalonia.ReactiveUI;
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

        return mainWindow;

    }
}
