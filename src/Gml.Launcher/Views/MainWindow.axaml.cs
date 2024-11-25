using System;
using Avalonia;
using Avalonia.Controls.Shapes;
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
        this.WhenActivated(_ => { });
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDevTools();
#endif
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is MainWindowViewModel viewModel)
            viewModel.GameLaunched.Subscribe(isActive =>
            {
                if (isActive)
                    Hide();
                else
                    Show();
            });
    }

    protected override void OnClosed(EventArgs e)
    {
        SendCloseEvent();

        base.OnClosed(e);
    }

    private void SendCloseEvent()
    {
        if (DataContext is MainWindowViewModel viewModel)
            viewModel.OnClosed.OnNext(false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.Source is Rectangle rectangle)
        {
            BeginMoveDrag(e);
        }
    }

    private void WindowClosed(object? sender, EventArgs e)
    {
        SendCloseEvent();
    }
}
