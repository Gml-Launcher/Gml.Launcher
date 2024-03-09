using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using GamerVII.Notification.Avalonia;
using Gml.Client;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Services;
using Gml.Launcher.Models;
using Splat;

namespace Gml.Launcher.Core.Extensions;

public static class ServiceLocator
{
    public static AppBuilder RegisterServices(this AppBuilder builder)
    {
        var systemService = new SystemService();

        var installationDirectory = Path.Combine(systemService.GetApplicationFolder(), ResourceKeysDictionary.FolderName);

        Locator.CurrentMutable.RegisterConstant(new ResourceLocalizationService(), typeof(ILocalizationService));
        Locator.CurrentMutable.RegisterConstant(systemService, typeof(ISystemService));
        Locator.CurrentMutable.RegisterConstant(new LocalStorageService(), typeof(IStorageService));
        Locator.CurrentMutable.RegisterConstant(new GmlClientManager(ResourceKeysDictionary.Host, installationDirectory), typeof(IGmlClientManager));

        return builder;
    }

}
