using Avalonia;
using Avalonia.Controls.Primitives;

namespace Gml.Launcher.Views.Components;

public class OnlineComponent : TemplatedControl
{
    public static readonly StyledProperty<string> OnlineProperty = AvaloniaProperty.Register<OnlineComponent, string>(
        "Online", "0");

    public string Online
    {
        get => GetValue(OnlineProperty);
        set => SetValue(OnlineProperty, value);
    }
}
