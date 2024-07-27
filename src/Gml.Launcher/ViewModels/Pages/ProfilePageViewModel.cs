using System;
using System.Reactive.Concurrency;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using ReactiveUI;

namespace Gml.Launcher.ViewModels.Pages;

public class ProfilePageViewModel : PageViewModelBase
{
    private IUser _user;
    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public IUser User
    {
        get => _user;
        set => this.RaiseAndSetIfChanged(ref _user, value);
    }

    internal ProfilePageViewModel(IScreen screen,
        IUser user,
        ILocalizationService? localizationService = null) : base(screen,
        localizationService)
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));

        RxApp.MainThreadScheduler.Schedule(LoadData);

    }

    private void LoadData()
    {
        // string logs = await _storageService.GetLogsAsync(1500);
        //
        // var exception = new Exception(logs);
        //
        // SentrySdk.CaptureException(exception);
    }
}
