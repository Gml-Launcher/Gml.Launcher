using Avalonia;
using Avalonia.Media;
using GamerVII.Notification.Avalonia;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using Gml.Launcher.ViewModels.Pages;
using ReactiveUI;
using Splat;
using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace Gml.Launcher.ViewModels;

public class MainWindowViewModel : WindowViewModelBase, IScreen
{
    private readonly IVpnChecker _vpnChecker;
    private readonly ILocalizationService _localizationService;
    private readonly IBackendChecker _backendChecker;
    protected internal readonly Subject<bool> _gameLaunched = new();

    public MainWindowViewModel(IVpnChecker? vpnChecker = null, ILocalizationService? localizationService = null, IBackendChecker? backendChecker = null)
    {
        _vpnChecker = vpnChecker ?? Locator.Current.GetService<IVpnChecker>()
            ?? throw new ServiceNotFoundException(typeof(IVpnChecker));

        _localizationService = localizationService ?? Locator.Current.GetService<ILocalizationService>()
            ?? throw new ServiceNotFoundException(typeof(ILocalizationService));

        _backendChecker = backendChecker ?? Locator.Current.GetService<IBackendChecker>()
            ?? throw new ServiceNotFoundException(typeof(IBackendChecker));

        Router.Navigate.Execute(new LoginPageViewModel(this, OnClosed));

        RxApp.MainThreadScheduler.Schedule(TimeSpan.FromSeconds(2), CheckBackend);

        RxApp.MainThreadScheduler.Schedule(TimeSpan.FromSeconds(4), CheckSystem);
    }

    public INotificationMessageManager Manager { get; } = new NotificationMessageManager();
    protected internal IObservable<bool> GameLaunched => _gameLaunched;
    public RoutingState Router { get; } = new();

    public static async void RestartApp()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = $"{Environment.ProcessPath}",
            Arguments = string.Empty,
            UseShellExecute = true
        });
        Environment.Exit(0);
    }

    public async void CheckBackendStatus(INotificationMessageButton button)
    {
        button.IsEnabled = false;
        button.Content = _localizationService.GetString(ResourceKeysDictionary.CheckBackendTitle);

        var CheckMessage = Manager
            .CreateMessage(false, "#222",
                _localizationService.GetString(ResourceKeysDictionary.CheckBackendTitle),
                _localizationService.GetString(ResourceKeysDictionary.CheckBackendMessage))
            .WithAdditionalContent(ContentLocation.Bottom, new Avalonia.Controls.ProgressBar
            {
                Foreground = new SolidColorBrush(Color.FromArgb(128, 200, 200, 200)),
                Background = Brushes.Black,
                Margin = new Thickness(5, 5, 5, 5),
                IsIndeterminate = true,
                IsHitTestVisible = false
            })
            .WithCloseButtonVisible(false)
            .Queue();

        bool backendInactive = await _backendChecker.BackendIsActive();

        Manager.Dismiss(CheckMessage);
        button.IsEnabled = true;
        button.Content = _localizationService.GetString(ResourceKeysDictionary.CheckBackendButton);

        if (!backendInactive)
        {
            Manager
                .CreateMessage(false, "#086",
                    _localizationService.GetString(ResourceKeysDictionary.CheckBackendTitle),
                    _localizationService.GetString(ResourceKeysDictionary.CheckBackendResultSuccessful)).Dismiss().WithDelay(TimeSpan.FromSeconds(5))
                .Queue();
        }
        else
        {
            Manager
               .CreateMessage(false, "#933",
                   _localizationService.GetString(ResourceKeysDictionary.CheckBackendTitle),
                   _localizationService.GetString(ResourceKeysDictionary.CheckBackendResultFailed)).Dismiss().WithDelay(TimeSpan.FromSeconds(5))
               .Queue();
        }
    }

    private async void CheckBackend()
    {
        if (_backendChecker.IsOffline)
        {
            Manager
                .CreateMessage(true, "#993030",
                    _localizationService.GetString(ResourceKeysDictionary.OfflineMode),
                    _localizationService.GetString(ResourceKeysDictionary.OfflineDescription))
                .HasBadge("WARNING")
                .Accent("#A54848")
                .WithButtonsVisibility(true)
                .WithCloseButtonVisible(true)
                .WithButton(_localizationService.GetString(ResourceKeysDictionary.RestartButton), button => RestartApp())
                .WithButton(_localizationService.GetString(ResourceKeysDictionary.CheckBackendButton), button => CheckBackendStatus(button))
                .Queue();
        }
    }
    private async void CheckSystem()
    {
        if (_vpnChecker.IsUseVpnTunnel())
        {
            Manager
                .CreateMessage(true, "#3684EA",
                    _localizationService.GetString(ResourceKeysDictionary.Information),
                    _localizationService.GetString(ResourceKeysDictionary.VpnUse))
                .Dismiss()
                .WithDelay(TimeSpan.FromSeconds(10))
                .Queue();
        }
    }
}
