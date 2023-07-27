using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GamerVII.Launcher.Views.Pages;

public partial class ClientSettingsPageView : UserControl
{
    public ClientSettingsPageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}