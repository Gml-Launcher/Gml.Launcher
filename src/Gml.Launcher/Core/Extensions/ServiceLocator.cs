using System;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using GamerVII.Notification.Avalonia;
using Gml.Client;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Services;
using Gml.Launcher.Models;
using ReactiveUI;
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
        Locator.CurrentMutable.RegisterConstant(new LocalStorageService(), typeof(IStorageService));

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
