using GamerVII.MinecraftLauncher.Models.Enums;

namespace GamerVII.MinecraftLauncher.Models.Client;

public class GameClient : IGameClient
{
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string Description { get; set; }
    public required object Image { get; set; }
    public ModLoaderType ModLoaderType { get; set; }
}
