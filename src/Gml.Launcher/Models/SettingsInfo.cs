namespace Gml.Launcher.Models;

public record SettingsInfo(
    int GameWidth,
    int GameHeight,
    bool FullScreen,
    bool IsDynamicRam,
    double RamValue,
    string? LanguageCode)
{
    public static SettingsInfo Default => new(900, 600, false, true, 0, "ru-RU");
}
