using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Gml.Client;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Services;
using Gml.Launcher.Models;
using Sentry;
using Splat;

namespace Gml.Launcher.Core.Extensions;

public static class ServiceLocator
{
    public static AppBuilder RegisterServices(this AppBuilder builder)
    {
        var systemService = new SystemService();

        var installationDirectory =
            Path.Combine(systemService.GetApplicationFolder(), ResourceKeysDictionary.FolderName);

        RegisterLocalizationService();
        RegisterSystemService(systemService);
        var manager = RegisterGmlManager(systemService, installationDirectory);
        var storageService = RegisterStorage();

        CheckAndChangeInstallationFolder(storageService, manager);
        CheckAndChangeLanguage(storageService, systemService);

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            SentrySdk.CaptureException((Exception)args.ExceptionObject);
        };

        return builder;
    }

    private static void CheckAndChangeLanguage(LocalStorageService storageService, SystemService systemService)
    {
        var data = storageService.GetAsync<SettingsInfo>(StorageConstants.Settings).Result;

        if (data != null && !string.IsNullOrEmpty(data.LanguageCode))
            Thread.CurrentThread.CurrentCulture = systemService
                .GetAvailableLanguages()
                .FirstOrDefault(c => c.Culture.Name == data.LanguageCode)?
                .Culture ?? new CultureInfo("ru-RU");
    }

    private static LocalStorageService RegisterStorage()
    {
        var storageService = new LocalStorageService();
        Locator.CurrentMutable.RegisterConstant(storageService, typeof(IStorageService));

        return storageService;
    }

    private static void CheckAndChangeInstallationFolder(LocalStorageService storageService, GmlClientManager manager)
    {
        var installationDirectory = storageService.GetAsync<string>(StorageConstants.InstallationDirectory).Result;

        if (!string.IsNullOrEmpty(installationDirectory)) manager.ChangeInstallationFolder(installationDirectory);
    }

    private static GmlClientManager RegisterGmlManager(SystemService systemService, string installationDirectory)
    {
        var manager = new GmlClientManager(installationDirectory, ResourceKeysDictionary.Host,
            ResourceKeysDictionary.FolderName,
            systemService.GetOsType());

        Locator.CurrentMutable.RegisterConstant(manager, typeof(IGmlClientManager));

        return manager;
    }

    private static void RegisterSystemService(SystemService systemService)
    {
        Locator.CurrentMutable.RegisterConstant(systemService, typeof(ISystemService));
    }

    private static void RegisterLocalizationService()
    {
        var service = new ResourceLocalizationService();
        Locator.CurrentMutable.RegisterConstant(service, typeof(ILocalizationService));
    }
}
