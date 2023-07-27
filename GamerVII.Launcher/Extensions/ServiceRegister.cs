using Avalonia;
using GamerVII.Launcher.Services.AuthService;
using GamerVII.Launcher.Services.ClientService;
using GamerVII.Launcher.Services.GameLaunchService;
using GamerVII.Launcher.Services.LocalStorage;
using GamerVII.Launcher.Services.LoggerService;
using Splat;

namespace GamerVII.Launcher.Extensions;

public static class ServiceRegister
{

    public static AppBuilder RegisterServices(this AppBuilder builder)
    {

        Locator.CurrentMutable.RegisterConstant(new FileLoggerService(), typeof(ILoggerService));
        Locator.CurrentMutable.RegisterConstant(new LocalStorageService(), typeof(ILocalStorageService));
        Locator.CurrentMutable.RegisterConstant(new LocalGameClientService(), typeof(IGameClientService));
        Locator.CurrentMutable.RegisterConstant(new GameLaunchService(), typeof(IGameLaunchService));
        Locator.CurrentMutable.RegisterConstant(new AuthService(), typeof(IAuthService));

        return builder;

    }

}
