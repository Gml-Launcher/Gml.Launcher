using System;
using System.Reactive;
using System.Reactive.Subjects;
using GamerVII.Notification.Avalonia;
using Gml.Launcher.ViewModels.Base;
using Gml.Launcher.ViewModels.Pages;
using ReactiveUI;

namespace Gml.Launcher.ViewModels;

public class MainWindowViewModel : WindowViewModelBase, IScreen
{
    public RoutingState Router { get; } = new();

    public ReactiveCommand<Unit, IRoutableViewModel> GoBackCommand => Router.NavigateBack!;

    public INotificationMessageManager Manager { get; } = new NotificationMessageManager();
    protected internal Subject<bool> gameLaunched = new();
    protected internal IObservable<bool> GameLaunched => gameLaunched;

    public MainWindowViewModel()
    {
        Router.Navigate.Execute(new LoginPageViewModel(this, OnClosed));
    }
}
