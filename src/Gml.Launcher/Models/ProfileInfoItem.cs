namespace Gml.Launcher.Models;

public class ProfileInfoItem
{
    public ProfileInfoItem(string key, string value)
    {
        Value = value;
        Key = key;
    }

    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}
