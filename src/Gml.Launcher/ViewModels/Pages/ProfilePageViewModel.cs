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
using ReactiveUI.Fody.Helpers;
using Sentry;

namespace Gml.Launcher.ViewModels.Pages;

public class ProfilePageViewModel : PageViewModelBase
{
    private readonly IGmlClientManager _manager;

    [Reactive] public string TextureUrl { get; set; }
    [Reactive] public IUser User { get; set; }
    internal ProfilePageViewModel(
        IScreen screen,
        IUser user,
        IGmlClientManager manager,
        ILocalizationService? localizationService = null) : base(screen,
        localizationService)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        _manager = manager;

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }

    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    private async void LoadData()
    {
        try
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] Loading texture data...]");
            var userTextureInfo = await _manager.GetTexturesByName(User.Name);

            if (userTextureInfo is null)
                return;

            TextureUrl = userTextureInfo.FullSkinUrl ?? string.Empty;
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss:fff}] Textures updated: {TextureUrl}");
        }
        catch (Exception exception)
        {
            SentrySdk.CaptureException(exception);
        }
    }

}
