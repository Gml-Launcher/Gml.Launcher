using Avalonia;
using Avalonia.Controls;

namespace Gml.Launcher.Views.Components;

public class StackFrameBorder : ItemsControl
{
    public static readonly StyledProperty<int> SpacingProperty = AvaloniaProperty.Register<StackFrameBorder, int>(
        nameof(Spacing));

    public int Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }
}
