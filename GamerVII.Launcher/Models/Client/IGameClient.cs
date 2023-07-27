using GamerVII.Launcher.Models.Enums;

namespace GamerVII.Launcher.Models.Client;
public interface IGameClient
{
    string Name { get; set; }
    string Version { get; set; }
    string InstallationVersion { get; set; }
    string Description { get; set; }
    object Image { get; set; }
    ModLoaderType ModLoaderType { get; set; }
}
