using Avalonia;
using Avalonia.Controls.Primitives;

namespace Gml.Launcher.Views.Components;

public class LogoComponent : TemplatedControl
{
    public static readonly StyledProperty<double> SizeProperty = AvaloniaProperty.Register<LogoComponent, double>(
        nameof(Size), 32);

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
}
