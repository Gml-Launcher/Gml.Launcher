using System.IO;
using Avalonia;
using GamerVII.Notification.Avalonia;
using Gml.Client;
using Gml.Launcher.Core.Services;
using Splat;

namespace Gml.Launcher.Core.Extensions;

public static class ServiceLocator
{
    public static AppBuilder RegisterServices(this AppBuilder builder)
    {
        var systemService = new SystemService();

        var baseAddress = "http://localhost:5000";
        var skinServiceAddress = "http://localhost:5006";
        var installationDirectory = Path.Combine(systemService.GetApplicationFolder(), "GamerVIILauncher"); // ToDo: to const

        Locator.CurrentMutable.RegisterConstant(new ResourceLocalizationService(), typeof(ILocalizationService));
        Locator.CurrentMutable.RegisterConstant(systemService, typeof(ISystemService));
        Locator.CurrentMutable.RegisterConstant(new LocalStorageService(), typeof(IStorageService));
        Locator.CurrentMutable.RegisterConstant(new GmlClientManager(baseAddress, skinServiceAddress, installationDirectory), typeof(IGmlClientManager));

        return builder;
    }
}
