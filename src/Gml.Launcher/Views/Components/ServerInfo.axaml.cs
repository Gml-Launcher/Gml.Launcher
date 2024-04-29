using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Gml.Launcher.Views.Components;

public class ServerInfo : TemplatedControl
{

    public static readonly StyledProperty<string> ProfileNameProperty = AvaloniaProperty.Register<ServerInfo, string>(
        nameof(ProfileName), "Techno Magic");

    public static readonly StyledProperty<string> StatusProperty = AvaloniaProperty.Register<ServerInfo, string>(
        nameof(Status), "Available");

    public static readonly StyledProperty<int> SpacingProperty = AvaloniaProperty.Register<ServerInfo, int>(
        nameof(Spacing));

    public static readonly StyledProperty<ICommand> PlayCommandProperty = AvaloniaProperty.Register<ServerInfo, ICommand>(
        nameof(PlayCommand));

    public static readonly StyledProperty<ICommand> SettingsCommandProperty = AvaloniaProperty.Register<ServerInfo, ICommand>(
        nameof(SettingsCommand));

    public static readonly StyledProperty<string> ProfileDescriptionProperty = AvaloniaProperty.Register<ServerInfo, string>(
        nameof(ProfileDescription));

    public string ProfileDescription
    {
        get => GetValue(ProfileDescriptionProperty);
        set => SetValue(ProfileDescriptionProperty, value);
    }

    public ICommand SettingsCommand
    {
        get => GetValue(SettingsCommandProperty);
        set => SetValue(SettingsCommandProperty, value);
    }

    public ICommand PlayCommand
    {
        get => GetValue(PlayCommandProperty);
        set => SetValue(PlayCommandProperty, value);
    }

    public int Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public string Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string ProfileName
    {
        get => GetValue(ProfileNameProperty);
        set => SetValue(ProfileNameProperty, value);
    }

}

