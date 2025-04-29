using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Gml.Launcher.Views.Components;

public class OutlinedTextBlock : Control
{
        public static readonly StyledProperty<string?> TextProperty =
            AvaloniaProperty.Register<OutlinedTextBlock, string?>(nameof(Text));

        public static readonly StyledProperty<FontFamily> FontFamilyProperty =
            AvaloniaProperty.Register<OutlinedTextBlock, FontFamily>(nameof(FontFamily), new FontFamily("Segoe UI"));

        public static readonly StyledProperty<double> FontSizeProperty =
            AvaloniaProperty.Register<OutlinedTextBlock, double>(nameof(FontSize), 12);

        public static readonly StyledProperty<IBrush> FillProperty =
            AvaloniaProperty.Register<OutlinedTextBlock, IBrush>(nameof(Fill), Brushes.White);

        public static readonly StyledProperty<IBrush> StrokeProperty =
            AvaloniaProperty.Register<OutlinedTextBlock, IBrush>(nameof(Stroke), Brushes.Black);

        public static readonly StyledProperty<double> StrokeThicknessProperty =
            AvaloniaProperty.Register<OutlinedTextBlock, double>(nameof(StrokeThickness), 1.0);


        static OutlinedTextBlock()
        {
            FillProperty.Changed.AddClassHandler<OutlinedTextBlock>((control, args) => control.InvalidateVisual());
        }

        public string? Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public FontFamily FontFamily
        {
            get => GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public double FontSize
        {
            get => GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public IBrush Fill
        {
            get => GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public IBrush Stroke
        {
            get => GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public double StrokeThickness
        {
            get => GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }


        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (string.IsNullOrEmpty(Text))
                return;

            var typeface = new Typeface(FontFamily);
            var formattedText = new FormattedText(
                Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                FontSize,
                Fill
            );

            var geometry = formattedText.BuildGeometry(new Point(0, 0));

            if (Stroke is ISolidColorBrush strokeBrush)
            {
                context.DrawGeometry(null, new Pen(strokeBrush, StrokeThickness), geometry);
            }

            context.DrawGeometry(Fill, null, geometry);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (string.IsNullOrEmpty(Text))
                return new Size(0, 0);

            var formattedText = new FormattedText(
                Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily),
                FontSize,
                Fill ?? Brushes.Black // Тут важно что-то не-null, иначе будет исключение
            );

            var geometry = formattedText.BuildGeometry(new Point(0, 0));
            var bounds = geometry.Bounds;

            // Добавим толщину обводки к размеру
            double thickness = StrokeThickness;
            return new Size(bounds.Width + thickness * 2, bounds.Height + thickness * 2);
        }
}
