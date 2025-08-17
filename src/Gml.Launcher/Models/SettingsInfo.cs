using System.Globalization;

namespace Gml.Launcher.Models;

public record SettingsInfo
{
    public SettingsInfo()
    {
    }

    public SettingsInfo(int gameWidth,
        int gameHeight,
        bool fullScreen,
        bool isDynamicRam,
        double ramValue,
        string? languageCode)
    {
        GameHeight = gameHeight;
        GameWidth = gameWidth;
        FullScreen = fullScreen;
        IsDynamicRam = isDynamicRam;
        RamValue = ramValue;
        LanguageCode = languageCode;
    }

    public int GameWidth { get; set; }
    public int GameHeight { get; set; }
    public bool FullScreen { get; set; }
    public bool IsDynamicRam { get; set; }
    public double RamValue { get; set; }
    public string? LanguageCode { get; set; }

    public static SettingsInfo Default =>
        new SettingsInfo(900, 600, false, true, 1024, CultureInfo.CurrentUICulture.Name);

    public static SettingsInfo Empty => new();
}
