using GamerVII.MinecraftLauncher.Models.Client;
using GamerVII.MinecraftLauncher.Services.Base;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GamerVII.MinecraftLauncher.Services.Launch;

interface IGameLaunchService : IServiceBase
{
    delegate void ProgressChangedEventHandler(decimal percentage);
    event ProgressChangedEventHandler ProgressChanged;

    delegate void FileChangedEventHandler(string percentage);
    event FileChangedEventHandler FileChanged;

    delegate void LoadClientEventHandler(bool loadClientEnded, string? message);
    event LoadClientEventHandler LoadClientEnded;

    Task<Process> LaunchClient(IGameClient client);
    Task<IGameClient> LoadClient(IGameClient client);
}
