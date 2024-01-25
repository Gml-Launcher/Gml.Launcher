using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

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

