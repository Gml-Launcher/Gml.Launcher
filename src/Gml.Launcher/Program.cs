using System;
using System.Diagnostics;
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
#if DEBUG
    public static void Main(string[] args)
    {
        try
        {
            Debug.WriteLine($"[Gml][{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application started");
            //InitializeSentry();
            // RxApp.DefaultExceptionHandler = Observer.Create<Exception>(GlobalExceptionHandler);
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception exception)
        {
            SentrySdk.CaptureException(exception);
            Console.WriteLine(exception);
        }
    }
#else
    public static void Main(string[] args)
    {
        try
        {
            Debug.WriteLine($"[Gml][{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application started");
            //InitializeSentry();
            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(GlobalExceptionHandler);
            BuildAvaloniaApp(args)
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception exception)
        {
            SentrySdk.CaptureException(exception);
            Console.WriteLine(exception);
        }
    }
#endif

    private static void GlobalExceptionHandler(Exception exception)
    {
        SentrySdk.CaptureException(exception);
    }

#if DEBUG
    public static AppBuilder BuildAvaloniaApp()
    {
        Debug.WriteLine($"[Gml][{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuring launcher");
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .RegisterServices([])
            .LogToTrace()
            .UseReactiveUI();
    }
#else
    public static AppBuilder BuildAvaloniaApp(string[] args)
    {
        Debug.WriteLine($"[Gml][{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuring launcher");
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .RegisterServices(args)
            .LogToTrace()
            .UseReactiveUI();
    }
#endif

    private static void InitializeSentry()
    {
        Debug.WriteLine($"[Gml][{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Start sentry initialization");
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

            Debug.WriteLine($"[Gml][{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Sentry initialized");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}
