using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;

namespace Gml.Launcher.Core.Converters;

public class Base64ToBitmapConverter : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string base64)
            return AvaloniaProperty.UnsetValue;

        try
        {
            var bytes = System.Convert.FromBase64String(base64);
            using var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }
        catch
        {
            // ignored
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
