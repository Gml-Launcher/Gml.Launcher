using System.Globalization;

namespace Gml.Launcher.Models;

public class Language
{
    public required string IconPath { get; init; }
    public required string Name { get; init; }
    public required CultureInfo Culture { get; init; }
}
