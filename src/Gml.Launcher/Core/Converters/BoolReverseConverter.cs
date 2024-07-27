using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Gml.Launcher.Core.Converters;

public class BoolReverseConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (bool.TryParse(value?.ToString(), out var boolValue)) return !boolValue;

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
