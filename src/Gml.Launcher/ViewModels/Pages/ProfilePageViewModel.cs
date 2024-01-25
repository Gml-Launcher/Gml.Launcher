using System.Reactive;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using ReactiveUI;

namespace Gml.Launcher.ViewModels.Pages;

public class ProfilePageViewModel : PageViewModelBase
{
    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public ReactiveCommand<Unit, IRoutableViewModel> GoBackCommand { get; set; }


    internal ProfilePageViewModel(IScreen screen, ILocalizationService? localizationService = null) : base(screen,
        localizationService)
    {
        GoBackCommand = screen.Router.NavigateBack!;
    }
}
