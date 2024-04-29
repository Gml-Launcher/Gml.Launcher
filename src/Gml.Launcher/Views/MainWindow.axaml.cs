using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Gml.Launcher.ViewModels;
using ReactiveUI;

namespace Gml.Launcher.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDevTools();
#endif
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }
}
