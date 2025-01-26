using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Gml.Launcher.ViewModels;
using Gml.Launcher.Views;
using Gml.Launcher.Views.SplashScreen;

namespace Gml.Launcher;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splashViewModel = new SplashScreenViewModel();
            var splashScreen = new SplashScreen
            {
                DataContext = splashViewModel
            };

            desktop.MainWindow = splashScreen;
            await splashViewModel.InitializeAsync();

            desktop.MainWindow = splashScreen.GetMainWindow();
            desktop.MainWindow.Show();
            splashScreen.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
