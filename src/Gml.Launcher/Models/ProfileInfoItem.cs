namespace Gml.Launcher.Models;

public class ProfileInfoItem(string key, string value)
{
    public string Key { get; set; } = key;
    public string Value { get; set; } = value;
}
