using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.Forge;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.LoggerService;
using Splat;

namespace GamerVII.Launcher.Services.GameLaunchService;

public class GameLaunchService : IGameLaunchService
{
    private readonly ILoggerService _loggerService;
    public CMLauncher Launcher { get; private set; }

    public event IGameLaunchService.ProgressChangedEventHandler ProgressChanged;
    public event IGameLaunchService.FileChangedEventHandler FileChanged;
    public event IGameLaunchService.LoadClientEventHandler LoadClientEnded;

    private readonly MinecraftPath _path;

    private delegate void ProgressChangedEventHandler(decimal percentage);

    private delegate void FileChangedEventHandler(string percentage);

    private delegate void LoadClientEventHandler(bool loadClientEnded, string? message);

    public GameLaunchService(ILoggerService? loggerService = null)
    {
        _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>()!;
        
        System.Net.ServicePointManager.DefaultConnectionLimit = 256;

        _path = new MinecraftPath();
        Launcher = new CMLauncher(_path);

        Launcher.ProgressChanged += (sender, e) => ProgressChanged?.Invoke(((decimal)e.ProgressPercentage) / 100);
        Launcher.FileChanged += (e) => FileChanged?.Invoke(e.FileName);
    }

    public async Task<Process> LaunchClient(IGameClient client, IUser user)
    {
        Process process = new Process();
        
        try
        {
            var session = new MSession(user.Login, user.AccessToken, "uuid");
            
            process = await Launcher.CreateProcessAsync(client.InstallationVersion, new MLaunchOption
            {
                MaximumRamMb = 4096,
                Session = session,
            }, true);

            process.EnableRaisingEvents = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            
            process.ErrorDataReceived += (s, e) => _loggerService.Log(e.Data);
            process.OutputDataReceived += (s, e) => _loggerService.Log(e.Data);
            
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }
        catch (Exception ex)
        {
            _loggerService.Log(ex.Message);
            _loggerService.Log(ex.StackTrace);
        }

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
            client.InstallationVersion = version.Id;

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

        client.InstallationVersion = await forge.Install(client.Version);
    }
}