using System.Reactive;
using Gml.Launcher.ViewModels.Base;
using Gml.Launcher.ViewModels.Pages;
using ReactiveUI;

namespace Gml.Launcher.ViewModels;

public class MainWindowViewModel : WindowViewModelBase, IScreen
{
    public RoutingState Router { get; } = new();

    public ReactiveCommand<Unit, IRoutableViewModel> GoBackCommand => Router.NavigateBack!;

    public MainWindowViewModel()
    {
        Router.Navigate.Execute(new LoginPageViewModel(this));
    }
}
