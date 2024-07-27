using System;
using System.Reactive.Subjects;
using GamerVII.Notification.Avalonia;
using Gml.Launcher.ViewModels.Base;
using Gml.Launcher.ViewModels.Pages;
using ReactiveUI;

namespace Gml.Launcher.ViewModels;

public class MainWindowViewModel : WindowViewModelBase, IScreen
{
    protected internal readonly Subject<bool> _gameLaunched = new();

    public MainWindowViewModel()
    {
        Router.Navigate.Execute(new LoginPageViewModel(this, OnClosed));
    }

    public INotificationMessageManager Manager { get; } = new NotificationMessageManager();
    protected internal IObservable<bool> GameLaunched => _gameLaunched;
    public RoutingState Router { get; } = new();
}
