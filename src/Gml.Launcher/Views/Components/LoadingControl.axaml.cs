using Avalonia;
using Avalonia.Controls.Primitives;

namespace Gml.Launcher.Views.Components;

public class LoadingControl : TemplatedControl
{
    public static readonly StyledProperty<double> SizeProperty = AvaloniaProperty.Register<LoadingControl, double>(
        nameof(Size), 16);

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
}
