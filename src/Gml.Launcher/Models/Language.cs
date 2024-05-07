using System.Globalization;

namespace Gml.Launcher.Models;

public class Language
{
    public required string IconPath { get; set; }
    public required string Name { get; set; }
    public required CultureInfo Culture { get; set; }
}
