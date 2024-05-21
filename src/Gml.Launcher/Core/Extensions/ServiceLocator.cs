using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
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

        var configurationManager = ConfigurationManager.OpenMachineConfiguration();

        var installationDirectory = Path.Combine(systemService.GetApplicationFolder(), ResourceKeysDictionary.FolderName);

        Locator.CurrentMutable.RegisterConstant(new ResourceLocalizationService(), typeof(ILocalizationService));
        Locator.CurrentMutable.RegisterConstant(systemService, typeof(ISystemService));
        Locator.CurrentMutable.RegisterConstant(new GmlClientManager(installationDirectory, ResourceKeysDictionary.Host, ResourceKeysDictionary.FolderName, systemService.GetOsType()), typeof(IGmlClientManager));

        var storageService = new LocalStorageService();
        Locator.CurrentMutable.RegisterConstant(storageService, typeof(IStorageService));

        var data = storageService.GetAsync<SettingsInfo>(StorageConstants.Settings).Result;

        if (data != null && !string.IsNullOrEmpty(data.LanguageCode))
        {
            Assets.Resources.Resources.Culture = systemService
                .GetAvailableLanguages()
                .FirstOrDefault(c => c.Culture.Name == data.LanguageCode)?
                .Culture;
        }

        Assets.Resources.Resources.Culture ??= new CultureInfo("ru-RU");

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            SentrySdk.CaptureException((Exception)args.ExceptionObject);
        };

        var sentryUrl = GmlClientManager.GetSentryLink(ResourceKeysDictionary.Host).Result;

        try
        {
            SentrySdk.Init(options =>
            {
                options.Dsn = sentryUrl;
                options.Debug = true;
                options.TracesSampleRate = 1.0;
                options.DiagnosticLevel = SentryLevel.Debug;
                options.IsGlobalModeEnabled = true;
                options.SendDefaultPii = true;
                options.MaxAttachmentSize = 10 * 1024 * 1024;
            });
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        return builder;
    }

}
