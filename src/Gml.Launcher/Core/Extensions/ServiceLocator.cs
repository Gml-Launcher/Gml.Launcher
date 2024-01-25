using System.IO;
using Avalonia;
using Gml.Client;
using Gml.Launcher.Core.Services;
using Splat;

namespace Gml.Launcher.Core.Extensions;

public static class ServiceLocator
{
    public static AppBuilder RegisterServices(this AppBuilder builder)
    {
        ISystemService systemService = new SystemService();

        var baseAddress = "http://localhost:5000";
        var installationDirectory = Path.Combine(systemService.GetApplicationFolder(), "GamerVIILauncher");// ToDo: to const


        Locator.CurrentMutable.RegisterConstant(new ResourceLocalizationService(), typeof(ILocalizationService));
        Locator.CurrentMutable.RegisterConstant(systemService, typeof(ISystemService));
        Locator.CurrentMutable.RegisterConstant(new GmlClientManager(baseAddress, installationDirectory), typeof(IGmlClientManager));

        return builder;
    }
}
