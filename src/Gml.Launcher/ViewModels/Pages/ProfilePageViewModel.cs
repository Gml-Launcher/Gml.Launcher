using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using Gml.Client;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using GmlCore.Interfaces;
using ReactiveUI;
using Sentry;

namespace Gml.Launcher.ViewModels.Pages;

public class ProfilePageViewModel : PageViewModelBase
{
    private IUser _user;
    private readonly IGmlClientManager _manager;

    internal ProfilePageViewModel(
        IScreen screen,
        IUser user,
        IGmlClientManager manager,
        ILocalizationService? localizationService = null) : base(screen,
        localizationService)
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _manager = manager;

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }

    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public IUser User
    {
        get => _user;
        set => this.RaiseAndSetIfChanged(ref _user, value);
    }

    private async void LoadData()
    {
        try
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] Loading texture data...]");
            var userTextureInfo = await _manager.GetTexturesByName(_user.Name);

            if (userTextureInfo is null)
                return;

            _user.TextureUrl = userTextureInfo.FullSkinUrl ?? string.Empty;

            this.RaisePropertyChanged(nameof(User));
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] Textures updated...]");
        }
        catch (Exception exception)
        {
            SentrySdk.CaptureException(exception);
        }
    }
}
