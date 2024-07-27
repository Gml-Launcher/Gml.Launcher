using System.IO;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Gml.Launcher.Views.Components;

public class BackgroundComponent : TemplatedControl
{
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<BackgroundComponent, IImage?>(
            nameof(Source));

    public static readonly StyledProperty<Stream?> SourceStreamProperty =
        AvaloniaProperty.Register<BackgroundComponent, Stream?>(
            nameof(SourceStream));

    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public Stream? SourceStream
    {
        get => GetValue(SourceStreamProperty);
        set => SetValue(SourceStreamProperty, value);
    }
}
