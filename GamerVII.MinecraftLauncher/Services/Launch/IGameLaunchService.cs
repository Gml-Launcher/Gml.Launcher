using GamerVII.MinecraftLauncher.Models.Client;
using GamerVII.MinecraftLauncher.Services.Base;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GamerVII.MinecraftLauncher.Services.Launch;

interface IGameLaunchService : IServiceBase
{
    delegate void LoadClientEventHandler(bool loadClientEnded, string? message);
    delegate void ProgressChangedEventHandler(decimal percentage);
    delegate void FileChangedEventHandler(string percentage);

    event LoadClientEventHandler LoadClientEnded;
    event ProgressChangedEventHandler ProgressChanged;
    event FileChangedEventHandler FileChanged;

    /// <summary>
    /// Launching a specific Minecraft client
    /// </summary>
    /// <param name="client">Minecraft client instance</param>
    /// <returns>The process responsible for launching the client</returns>
    Task<Process> LaunchClient(IGameClient client);

    /// <summary>
    /// Downloading a minecraft client of a certain version
    /// </summary>
    /// <param name="client">Minecraft client instance</param>
    /// <returns>Client information required to run</returns>
    Task<IGameClient> LoadClient(IGameClient client);
}
