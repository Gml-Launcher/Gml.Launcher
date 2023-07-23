using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using GamerVII.MinecraftLauncher.Models.Client;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GamerVII.MinecraftLauncher.Services.Launch;

class GameLaunchService : IGameLaunchService
{
    public CMLauncher Launcher { get; private set; }

    public event IGameLaunchService.ProgressChangedEventHandler ProgressChanged;
    public event IGameLaunchService.FileChangedEventHandler FileChanged;
    public event IGameLaunchService.LoadClientEventHandler LoadClientEnded;

    private readonly MinecraftPath _path;

    private delegate void ProgressChangedEventHandler(decimal percentage);
    private delegate void FileChangedEventHandler(string percentage);
    private delegate void LoadClientEventHandler(bool loadClientEnded, string? message);

    public GameLaunchService()
    {
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;

        _path = new MinecraftPath();
        Launcher = new CMLauncher(_path);

        Launcher.ProgressChanged += (sender, e) => ProgressChanged?.Invoke(((decimal)e.ProgressPercentage) / 100 );
        Launcher.FileChanged += (e) => FileChanged?.Invoke(e.FileName);
        
    }

    public async Task<Process> LaunchClient(IGameClient client)
    {
        var process = await Launcher.CreateProcessAsync(client.Version, new MLaunchOption
        {
            MaximumRamMb = 2048,
            Session = MSession.GetOfflineSession("hello123"),
        });

        process.Start();

        return process;
    }

    public Task<IGameClient> LoadClient(IGameClient client)
    {
        var thread = new Thread(() => LoadClientFiles(client));

        thread.IsBackground = true;

        thread.Start();

        return Task.FromResult(client);
    }

    private async void LoadClientFiles(IGameClient client)
    {
        try
        {
            var version = await Launcher.GetVersionAsync(client.Version);

            switch (client.ModLoaderType)
            {
                case Models.Enums.ModLoaderType.Vanilla:
                    await Launcher.CheckAndDownloadAsync(version);
                    break;

                case Models.Enums.ModLoaderType.Forge:
                    await LoadForge(client);
                    break;

                case Models.Enums.ModLoaderType.Fabric:
                    break;
                case Models.Enums.ModLoaderType.LiteLoader:
                    break;
                default:
                    break;
            }

            LoadClientEnded?.Invoke(true, "success");
        }
        catch (Exception ex)
        {
            LoadClientEnded?.Invoke(false, ex.Message);
        }
        
    }

    private async Task LoadForge(IGameClient client)
    {
        var forge = new MForge(Launcher);
        forge.FileChanged += (e) => FileChanged?.Invoke(e.FileName);
        forge.ProgressChanged += (sender, e) => ProgressChanged?.Invoke(((decimal)e.ProgressPercentage) / 100);

        client.Version = await forge.Install(client.Version);
    }
}
