using Avalonia;
using Avalonia.Controls.Primitives;

namespace Gml.Launcher.Views.Components;

public class ProgressBar : TemplatedControl
{
    public static readonly StyledProperty<string> PercentageProperty = AvaloniaProperty.Register<ProgressBar, string>(
        nameof(Percentage), "50");

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<ProgressBar, string>(
        nameof(Description), "Загружено: 1596 / 6599");

    public static readonly StyledProperty<string> HeadlineProperty = AvaloniaProperty.Register<ProgressBar, string>(
        nameof(Headline), "Обновление");

    public static readonly StyledProperty<string> SpeedProperty = AvaloniaProperty.Register<ProgressBar, string>(
        nameof(Speed), string.Empty);

    public string Headline
    {
        get => GetValue(HeadlineProperty);
        set => SetValue(HeadlineProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string Percentage
    {
        get => GetValue(PercentageProperty);
        set => SetValue(PercentageProperty, value);
    }

    public string Speed
    {
        get => GetValue(SpeedProperty);
        set => SetValue(SpeedProperty, value);
    }
}
