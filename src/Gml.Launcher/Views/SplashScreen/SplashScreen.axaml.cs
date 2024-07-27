using Avalonia.Controls;
using Gml.Launcher.ViewModels;

namespace Gml.Launcher.Views.SplashScreen;

public partial class SplashScreen : Window
{
    public SplashScreen()
    {
        InitializeComponent();
    }

    public MainWindow GetMainWindow()
    {
        return new MainWindow
        {
            DataContext = new MainWindowViewModel()
        };
    }
}
