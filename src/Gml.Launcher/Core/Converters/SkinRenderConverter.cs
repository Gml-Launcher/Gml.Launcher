using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Splat;

namespace Gml.Launcher.Core.Converters;

public class SkinRenderConverter : MarkupExtension, IValueConverter
{

    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        Bitmap? imageBitmap = default;

        var url = value?.ToString();

        if (!string.IsNullOrEmpty(url) && ValidateUrl(url))
        {
            using (var client = new HttpClient())
            {
                var response = client.GetByteArrayAsync(url).GetAwaiter().GetResult();
                using (var stream = new MemoryStream(response))
                {
                    return new Bitmap(new MemoryStream(SkinViewer.GetFront(stream, 128)));
                }
            }

        }

        return imageBitmap;
    }

    private bool ValidateUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
