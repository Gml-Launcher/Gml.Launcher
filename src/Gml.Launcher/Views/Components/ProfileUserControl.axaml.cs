using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Gml.Launcher.Models;

namespace Gml.Launcher.Views.Components;

public class ProfileUserControl : ItemsControl
{
    public static readonly StyledProperty<int> SpacingProperty = AvaloniaProperty.Register<StackFrameBorder, int>(
        nameof(Spacing));

    public static readonly StyledProperty<IEnumerable<ProfileInfoItem>> ProfileInfoItemsProperty =
        AvaloniaProperty.Register<ProfileUserControl, IEnumerable<ProfileInfoItem>>(
            nameof(ProfileInfoItems));

    public static readonly StyledProperty<string> SkinUrlProperty =
        AvaloniaProperty.Register<ProfileUserControl, string>(
            nameof(SkinUrl), "https://www.clipartkey.com/mpngs/m/215-2156859_fond-transparent-steve-minecraft.png");

    public static readonly StyledProperty<string> UserNameProperty =
        AvaloniaProperty.Register<ProfileUserControl, string>(
            nameof(UserName), "GamerVII");

    public string UserName
    {
        get => GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    public string SkinUrl
    {
        get => GetValue(SkinUrlProperty);
        set => SetValue(SkinUrlProperty, value);
    }

    public IEnumerable<ProfileInfoItem> ProfileInfoItems
    {
        get => GetValue(ProfileInfoItemsProperty);
        set => SetValue(ProfileInfoItemsProperty, value);
    }

    public int Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }
}
