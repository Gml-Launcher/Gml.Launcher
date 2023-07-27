using System.Diagnostics;
using System.Threading.Tasks;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Users;

namespace GamerVII.Launcher.Services.GameLaunchService;

public interface IGameLaunchService
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
    /// <param name="user"></param>
    /// <returns>The process responsible for launching the client</returns>
    Task<Process> LaunchClient(IGameClient client, IUser user);

    /// <summary>
    /// Downloading a minecraft client of a certain version
    /// </summary>
    /// <param name="client">Minecraft client instance</param>
    /// <returns>Client information required to run</returns>
    Task<IGameClient> LoadClient(IGameClient client);
}