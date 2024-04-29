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
    private IUser _user;
    private readonly IGmlClientManager _clientManager;
    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public IUser User
    {
        get => _user;
        private init => this.RaiseAndSetIfChanged(ref _user, value);
    }



    internal ProfilePageViewModel(IScreen screen,
        IUser user,
        IGmlClientManager? clientManager = null,
        ILocalizationService? localizationService = null) : base(screen,
        localizationService)
    {
        User = user;
        _clientManager = clientManager
                         ?? Locator.Current.GetService<IGmlClientManager>()
                         ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }

    private async void LoadData()
    {

    }
}
