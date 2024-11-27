using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using GamerVII.Notification.Avalonia;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using Gml.Launcher.ViewModels.Pages;
using ReactiveUI;
using Splat;

namespace Gml.Launcher.ViewModels;

public class MainWindowViewModel : WindowViewModelBase, IScreen
{
    private readonly IVpnChecker _vpnChecker;
    private readonly ILocalizationService _localizationService;
    protected internal readonly Subject<bool> _gameLaunched = new();

    public MainWindowViewModel(IVpnChecker? vpnChecker = null, ILocalizationService? localizationService = null)
    {
        _vpnChecker = vpnChecker ?? Locator.Current.GetService<IVpnChecker>()
            ?? throw new ServiceNotFoundException(typeof(IVpnChecker));

        _localizationService = localizationService ?? Locator.Current.GetService<ILocalizationService>()
            ?? throw new ServiceNotFoundException(typeof(ILocalizationService));

        Router.Navigate.Execute(new LoginPageViewModel(this, OnClosed));

        RxApp.MainThreadScheduler.Schedule(TimeSpan.FromSeconds(4), CheckSystem);
    }

    public INotificationMessageManager Manager { get; } = new NotificationMessageManager();
    protected internal IObservable<bool> GameLaunched => _gameLaunched;
    public RoutingState Router { get; } = new();

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
