using GamerVII.MinecraftLauncher.Models.Enums;

namespace GamerVII.MinecraftLauncher.Models.Client;

public interface IGameClient
{
    string Name { get; set; }
    string Version { get; set; }
    string Description { get; set; }
    object Image { get; set; }
    ModLoaderType ModLoaderType { get; set; }
}
