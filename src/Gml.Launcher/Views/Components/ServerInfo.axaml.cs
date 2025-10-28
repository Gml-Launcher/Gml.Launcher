using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using GmlCore.Interfaces.Enums;

namespace Gml.Launcher.Views.Components;

public class ServerInfo : TemplatedControl
{
    public static readonly StyledProperty<string> ProfileNameProperty = AvaloniaProperty.Register<ServerInfo, string>(
        nameof(ProfileName), "Techno Magic");

    public static readonly StyledProperty<string> StatusProperty = AvaloniaProperty.Register<ServerInfo, string>(
        nameof(Status), "Available");

    public static readonly StyledProperty<int> SpacingProperty = AvaloniaProperty.Register<ServerInfo, int>(
        nameof(Spacing));

    public static readonly StyledProperty<ICommand> PlayCommandProperty =
        AvaloniaProperty.Register<ServerInfo, ICommand>(
            nameof(PlayCommand));

    public static readonly StyledProperty<ICommand> SettingsCommandProperty =
        AvaloniaProperty.Register<ServerInfo, ICommand>(
            nameof(SettingsCommand));

    public static readonly StyledProperty<string> ProfileDescriptionProperty =
        AvaloniaProperty.Register<ServerInfo, string>(
            nameof(ProfileDescription));

    public static readonly StyledProperty<string> GameVersionProperty = AvaloniaProperty.Register<ServerInfo, string>(
        nameof(GameVersion));

    public static readonly StyledProperty<string> LaunchVersionProperty = AvaloniaProperty.Register<ServerInfo, string>(
        nameof(LaunchVersion));

    public static readonly StyledProperty<ProfileState> StateProperty = AvaloniaProperty.Register<ServerInfo, ProfileState>(
        nameof(State));

    public static readonly StyledProperty<DateTimeOffset?> CreateDateProperty = AvaloniaProperty.Register<ServerInfo, DateTimeOffset?>(
        nameof(CreateDate));

    public static readonly StyledProperty<ICommand> GoModsCommandProperty = AvaloniaProperty.Register<ServerInfo, ICommand>(
        nameof(GoModsCommand));

    public static readonly StyledProperty<bool> BackendIsNotOfflineProperty = AvaloniaProperty.Register<ServerInfo, bool>(
        nameof(BackendIsNotOffline));

    public static readonly StyledProperty<bool> IsModsButtonVisibleProperty =
        AvaloniaProperty.Register<ServerInfo, bool>(nameof(IsModsButtonVisible), defaultValue: true);

    public bool BackendIsNotOffline
    {
        get => GetValue(BackendIsNotOfflineProperty);
        set => SetValue(BackendIsNotOfflineProperty, value);
    }

    public bool IsModsButtonVisible
    {
        get => GetValue(IsModsButtonVisibleProperty);
        set => SetValue(IsModsButtonVisibleProperty, value);
    }

    public ICommand? GoModsCommand
    {
        get => GetValue(GoModsCommandProperty);
        set => SetValue(GoModsCommandProperty, value);
    }

    public DateTimeOffset? CreateDate
    {
        get => GetValue(CreateDateProperty);
        set => SetValue(CreateDateProperty, value);
    }

    public ProfileState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public string LaunchVersion
    {
        get => GetValue(LaunchVersionProperty);
        set => SetValue(LaunchVersionProperty, value);
    }

    public string GameVersion
    {
        get => GetValue(GameVersionProperty);
        set => SetValue(GameVersionProperty, value);
    }

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
