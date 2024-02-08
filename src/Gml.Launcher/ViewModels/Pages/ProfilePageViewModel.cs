using System.Reactive;
using System.Reactive.Concurrency;
using Gml.Client;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace Gml.Launcher.ViewModels.Pages;

public class ProfilePageViewModel : PageViewModelBase
{
    private readonly IUser _user;
    private readonly IGmlClientManager _clientManager;
    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);
    public IUser User => _user;

    public ReactiveCommand<Unit, IRoutableViewModel> GoBackCommand { get; set; }

    public string SkinUrl
    {
        get => _skinUrl;
        set => this.RaiseAndSetIfChanged(ref _skinUrl, value);
    }


    private string _skinUrl = string.Empty;

    internal ProfilePageViewModel(IScreen screen,
        IUser user,
        IGmlClientManager? clientManager = null,
        ILocalizationService? localizationService = null) : base(screen,
        localizationService)
    {
        _user = user;
        _clientManager = clientManager
                         ?? Locator.Current.GetService<IGmlClientManager>()
                         ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        GoBackCommand = screen.Router.NavigateBack!;

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }

    private async void LoadData()
    {
        SkinUrl = await _clientManager.GetSkinPath(_user);
    }
}
