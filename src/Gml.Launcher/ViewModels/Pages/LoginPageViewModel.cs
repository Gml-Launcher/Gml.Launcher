using System.Reactive;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using ReactiveUI;

namespace Gml.Launcher.ViewModels.Pages;

public class LoginPageViewModel : PageViewModelBase
{
    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public ReactiveCommand<Unit, IRoutableViewModel> LoginCommand { get; set; }


    internal LoginPageViewModel(
        IScreen screen,
        ILocalizationService? localizationService = null) : base(screen, localizationService)
    {

        LoginCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new OverviewPageViewModel(screen))
        );
    }
}
