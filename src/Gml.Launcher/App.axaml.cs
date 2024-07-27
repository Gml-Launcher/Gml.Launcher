using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Gml.Launcher.ViewModels;
using Gml.Launcher.Views;

namespace Gml.Launcher;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
#if DEBUG
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
#else
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
#endif
        }


        base.OnFrameworkInitializationCompleted();
    }
}
