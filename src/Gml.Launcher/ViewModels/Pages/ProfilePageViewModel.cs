using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reactive;
using System.Reactive.Concurrency;
using Gml.Client;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using ReactiveUI;
using Sentry;
using Splat;

namespace Gml.Launcher.ViewModels.Pages;

public class ProfilePageViewModel : PageViewModelBase
{
    private IUser _user;
    private readonly IGmlClientManager _clientManager;
    private readonly IStorageService _storageService;
    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public IUser User
    {
        get => _user;
        private init => this.RaiseAndSetIfChanged(ref _user, value);
    }



    internal ProfilePageViewModel(IScreen screen,
        IUser user,
        IGmlClientManager? clientManager = null,
        IStorageService? storageService = null,
        ILocalizationService? localizationService = null) : base(screen,
        localizationService)
    {
        User = user;
        _clientManager = clientManager
                         ?? Locator.Current.GetService<IGmlClientManager>()
                         ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        _storageService = storageService
                          ?? Locator.Current.GetService<IStorageService>()
                          ?? throw new ServiceNotFoundException(typeof(IStorageService));

        RxApp.MainThreadScheduler.Schedule(LoadData);



        // File.Delete("logs.txt");

    }

    private async void LoadData()
    {
        // string logs = await _storageService.GetLogsAsync(1500);
        //
        // var exception = new Exception(logs);
        //
        // SentrySdk.CaptureException(exception);
    }
}
