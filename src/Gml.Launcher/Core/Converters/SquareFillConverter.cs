using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Gml.Launcher.Core.Converters;

public class SquareFillConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values[0] is double value &&
            values[1] is double max &&
            parameter is string indexStr &&
            int.TryParse(indexStr, out int squareIndex))
        {
            double ratio = value / max;
            return squareIndex < ratio * 5;
        }
        return false;
    }
}
