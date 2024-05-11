using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Gml.Launcher.Views.Components;

public class BackgroundComponent : TemplatedControl
{

    public static readonly StyledProperty<IImage?> SourceProperty = AvaloniaProperty.Register<BackgroundComponent, IImage?>(
        nameof(Source));

    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

}

