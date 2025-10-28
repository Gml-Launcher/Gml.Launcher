using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Gml.Dto.Mods;
using Gml.Launcher.ViewModels;
using Gml.Launcher.ViewModels.Pages;

namespace Gml.Launcher.Core.Converters;

public class ModsDescriptionConverter : MarkupExtension, IMultiValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.FirstOrDefault() is ModReadDto modDto &&
            values.LastOrDefault() is ModsPageViewModel modsPageViewModel &&
            modsPageViewModel.OptionalModsDetails.TryGetValue($"{modDto.Name}.jar", out var modInfo))
        {
            return modInfo.Description;
        }

        return "Не указано";
    }
}
