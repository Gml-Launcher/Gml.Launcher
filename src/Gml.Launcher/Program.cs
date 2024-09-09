using System;
using System.Reactive;
using Avalonia;
using Avalonia.ReactiveUI;
using Gml.Client;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Extensions;
using ReactiveUI;
using Sentry;

namespace Gml.Launcher;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            InitializeSentry();
            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(GlobalExceptionHandler);
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception exception)
        {
            SentrySdk.CaptureException(exception);
            Console.WriteLine(exception);
        }
    }

    private static void GlobalExceptionHandler(Exception exception)
    {
        SentrySdk.CaptureException(exception);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .RegisterServices()
            .LogToTrace()
            .UseReactiveUI();
    }



    private static void InitializeSentry()
    {
        var sentryUrl = GmlClientManager.GetSentryLink(ResourceKeysDictionary.Host).Result;

        try
        {
            if (!string.IsNullOrEmpty(sentryUrl))
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
    }
}
