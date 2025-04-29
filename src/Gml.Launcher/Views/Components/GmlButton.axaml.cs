using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace Gml.Launcher.Views.Components;

public class GmlButton : TemplatedControl
{
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<GmlButton, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

    public static readonly StyledProperty<Uri> IconPathProperty = AvaloniaProperty.Register<GmlButton, Uri>(
        nameof(IconPath));

    public static readonly StyledProperty<double> IconSizeProperty = AvaloniaProperty.Register<GmlButton, double>(
        nameof(IconSize), 16);

    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<GmlButton, ICommand>(
        nameof(Command));

    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<GmlButton, string?>(
        nameof(Text), "DefaultButton style");

    public static readonly StyledProperty<int> SpacingProperty = AvaloniaProperty.Register<GmlButton, int>(
        nameof(Spacing), 10);

    public static readonly StyledProperty<bool> IsDefaultProperty = AvaloniaProperty.Register<GmlButton, bool>(
        nameof(IsDefault));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<GmlButton, object?>(
            nameof(CommandParameter));

    public static readonly StyledProperty<Color> ShadowColorProperty = AvaloniaProperty.Register<GmlButton, Color>(
        nameof(ShadowColor), Colors.Black);

    public static readonly StyledProperty<BoxShadows> InnerShadowProperty =
        AvaloniaProperty.Register<GmlButton, BoxShadows>(
            nameof(InnerShadow));


    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool IsDefault
    {
        get => GetValue(IsDefaultProperty);
        set => SetValue(IsDefaultProperty, value);
    }

    public int Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public Uri IconPath
    {
        get => GetValue(IconPathProperty);
        set => SetValue(IconPathProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public Color ShadowColor
    {
        get => GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    public BoxShadows InnerShadow
    {
        get => GetValue(InnerShadowProperty);
        set => SetValue(InnerShadowProperty, value);
    }

    public GmlButton()
    {
        UpdateInnerShadow();

        this.GetObservable(ShadowColorProperty).Subscribe(_ => UpdateInnerShadow());
    }

    private void UpdateInnerShadow()
    {
        InnerShadow = new BoxShadows(new BoxShadow
        {
            Color = ShadowColor,
            OffsetY = -5,
            Blur = 0,
            Spread = 0,
            IsInset = true
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (this.GetTemplateChildren().First() is Button button)
            button.Click += (_, _) => RaiseEvent(new RoutedEventArgs(ClickEvent));;
    }
}
