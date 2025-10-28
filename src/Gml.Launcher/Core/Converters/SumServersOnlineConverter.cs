using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Gml.Dto.Servers;

namespace Gml.Launcher.Core.Converters;

public class SumServersOnlineConverter : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is List<ServerReadDto> server)
            return server
                .Where(c => c.IsOnline)
                .Sum(c => c.Online)?
                .ToString() ?? "0";

        return "0";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "0";
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
