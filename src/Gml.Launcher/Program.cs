using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Reactive;
using Gml.Launcher.Core.Extensions;
using ReactiveUI;
using Sentry;

namespace Gml.Launcher;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(GlobalExceptionHandler);
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void GlobalExceptionHandler(Exception exception)
    {

        SentrySdk.CaptureException(exception);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .RegisterServices()
            .LogToTrace()
            .UseReactiveUI();
}
