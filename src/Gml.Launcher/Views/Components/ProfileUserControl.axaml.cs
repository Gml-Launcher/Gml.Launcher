using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Gml.Launcher.Models;

namespace Gml.Launcher.Views.Components;

public class ProfileUserControl : TemplatedControl
{

    public static readonly StyledProperty<IEnumerable<ProfileInfoItem>> ProfileInfoItemsProperty =
        AvaloniaProperty.Register<ProfileUserControl, IEnumerable<ProfileInfoItem>>(
            nameof(ProfileInfoItems));

    public static readonly StyledProperty<string> SkinUrlProperty = AvaloniaProperty.Register<ProfileUserControl, string>(
        nameof(SkinUrl));

    public static readonly StyledProperty<string> UserNameProperty = AvaloniaProperty.Register<ProfileUserControl, string>(
        nameof(UserName));

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

}
