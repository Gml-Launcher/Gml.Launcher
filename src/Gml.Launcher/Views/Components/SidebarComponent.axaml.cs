using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Gml.Launcher.ViewModels.Components;

namespace Gml.Launcher.Views.Components;

public class SidebarComponent : TemplatedControl
{
    public static readonly StyledProperty<bool> IsBackendInactiveProperty =
        AvaloniaProperty.Register<SidebarComponent, bool>(
            nameof(IsBackendInactive), false);

    public static readonly StyledProperty<ICommand> ProfileCommandProperty =
        AvaloniaProperty.Register<SidebarComponent, ICommand>(
            nameof(ProfileCommand));

    public static readonly StyledProperty<ICommand> LogoutCommandProperty =
        AvaloniaProperty.Register<SidebarComponent, ICommand>(
            nameof(LogoutCommand));

    public static readonly StyledProperty<ListViewModel> ListViewModelProperty =
        AvaloniaProperty.Register<SidebarComponent, ListViewModel>(
            nameof(ListViewModel));

    public static readonly StyledProperty<ICommand> HomeCommandProperty =
        AvaloniaProperty.Register<SidebarComponent, ICommand>(
            nameof(HomeCommand));

    public ICommand HomeCommand
    {
        get => GetValue(HomeCommandProperty);
        set => SetValue(HomeCommandProperty, value);
    }

    public bool IsBackendInactive
    {
        get => GetValue(IsBackendInactiveProperty);
    }

    public ListViewModel ListViewModel
    {
        get => GetValue(ListViewModelProperty);
        set => SetValue(ListViewModelProperty, value);
    }

    public ICommand LogoutCommand
    {
        get => GetValue(LogoutCommandProperty);
        set => SetValue(LogoutCommandProperty, value);
    }

    public ICommand ProfileCommand
    {
        get => GetValue(ProfileCommandProperty);
        set => SetValue(ProfileCommandProperty, value);
    }
}
