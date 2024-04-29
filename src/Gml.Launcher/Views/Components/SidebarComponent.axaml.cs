using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Gml.Launcher.ViewModels.Components;
using Gml.WebApi.Models.Dtos.Profiles;

namespace Gml.Launcher.Views.Components;

public class SidebarComponent : TemplatedControl
{

    public static readonly StyledProperty<ICommand> ProfileCommandProperty = AvaloniaProperty.Register<SidebarComponent, ICommand>(
        nameof(ProfileCommand));

    public static readonly StyledProperty<ICommand> LogoutCommandProperty = AvaloniaProperty.Register<SidebarComponent, ICommand>(
        nameof(LogoutCommand));

    public static readonly StyledProperty<ListViewModel> ListViewModelProperty = AvaloniaProperty.Register<SidebarComponent, ListViewModel>(
        nameof(ListViewModel));

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

