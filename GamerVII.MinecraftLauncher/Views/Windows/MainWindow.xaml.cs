using System.Windows;
using System.Windows.Input;

namespace GamerVII.MinecraftLauncher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void WindowMove(object sender, MouseButtonEventArgs e)
    {
        //if (e.LeftButton == MouseButtonState.Pressed)
        //{
        //    this.DragMove();
        //}
    }
}
