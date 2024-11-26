using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Gml.Launcher.Models;

public class Language
{
    public Language()
    {
    }

    [SetsRequiredMembers]
    public Language(string iconPath, string name, CultureInfo culture)
    {
        IconPath = iconPath;
        Name = name;
        Culture = culture;
    }

    public required string IconPath { get; init; }
    public required string Name { get; init; }
    public required CultureInfo Culture { get; init; }
}
