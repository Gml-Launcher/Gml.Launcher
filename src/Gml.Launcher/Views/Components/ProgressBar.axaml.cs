﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Gml.Launcher.Views.Components;

public class ProgressBar : TemplatedControl
{

    public static readonly StyledProperty<string> PercentageProperty = AvaloniaProperty.Register<ProgressBar, string>(
        nameof(Percentage), "50");

    public static readonly StyledProperty<string> DescriptionProperty = AvaloniaProperty.Register<ProgressBar, string>(
        nameof(Description), "Hitech");

    public static readonly StyledProperty<string> HeadlineProperty = AvaloniaProperty.Register<ProgressBar, string>(
        nameof(Headline), "Обновление");

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

}

