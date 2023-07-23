using GamerVII.MinecraftLauncher.Services.Auth;
using GamerVII.MinecraftLauncher.Services.ClientService;
using GamerVII.MinecraftLauncher.Services.Launch;
using GamerVII.MinecraftLauncher.Services.Skin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace GamerVII.MinecraftLauncher;

public partial class App : Application
{

    public static IHost? AppHost { get; private set; }


    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<MainWindow>();
                services.AddTransient<IGameClientService, LocalGameClientService>();
                services.AddTransient<ISkinService, SkinService>();
                services.AddTransient<IGameLaunchService, GameLaunchService>();
                services.AddTransient<IAuthService, AuthService>();
            })
            .Build();
    }



    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();

        base.OnExit(e);
    }

}
