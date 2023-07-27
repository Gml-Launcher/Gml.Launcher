using Avalonia;
using Avalonia.ReactiveUI;
using GamerVII.Launcher.Extensions;
using Splat;
using System;

namespace GamerVII.Launcher
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);


        public static AppBuilder BuildAvaloniaApp()
        {

            var app = AppBuilder.Configure<App>()
                                .UsePlatformDetect()
                                .WithInterFont()
                                .LogToTrace()
                                .RegisterServices()
                                .UseReactiveUI();



            return app; 

        }
    }
}