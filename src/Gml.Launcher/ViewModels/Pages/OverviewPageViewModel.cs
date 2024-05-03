using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using GamerVII.Notification.Avalonia;
using Gml.Client;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using Gml.Launcher.ViewModels.Components;
using Gml.Web.Api.Domains.System;
using Gml.Web.Api.Dto.Profile;
using ReactiveUI;
using Splat;

namespace Gml.Launcher.ViewModels.Pages;

public class OverviewPageViewModel : PageViewModelBase
{
    private readonly IScreen _screen;
    private readonly IStorageService _storageService;
    private readonly IGmlClientManager _clientManager;
    private readonly IUser _user;
    private int _loadingPercentage;
    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public ICommand GoProfileCommand { get; set; }
    public ICommand LogoutCommand { get; set; }
    public ICommand PlayCommand { get; set; }
    public ICommand GoSettingsCommand { get; set; }
    public ListViewModel ListViewModel { get; } = new();
    public IUser User => _user;

    public int LoadingPercentage
    {
        get => _loadingPercentage;
        set => this.RaiseAndSetIfChanged(ref _loadingPercentage, value);
    }

    internal OverviewPageViewModel(
        IScreen screen,
        IUser user,
        IGmlClientManager? clientManager = null,
        IStorageService? storageService = null) : base(screen)
    {
        _screen = screen;
        _user = user;
        _storageService = storageService
                          ?? Locator.Current.GetService<IStorageService>()
                          ?? throw new ServiceNotFoundException(typeof(IStorageService));

        _clientManager = clientManager
                         ?? Locator.Current.GetService<IGmlClientManager>()
                         ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        GoProfileCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new ProfilePageViewModel(screen, _user, clientManager))
        );

        GoSettingsCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new SettingsPageViewModel(screen, LocalizationService, _storageService, ListViewModel.SelectedProfile!))
        );

        _clientManager.ProgressChanged += (sender, args) =>
        {
            if (_loadingPercentage != args.ProgressPercentage)
            {
                LoadingPercentage = args.ProgressPercentage;
            }
        };

        LogoutCommand = ReactiveCommand.CreateFromTask(OnLogout);

        PlayCommand = ReactiveCommand.CreateFromTask(StartGame);

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }

    private async Task OnLogout(CancellationToken arg)
    {
        await _storageService.SetAsync(StorageConstants.User, new AuthUser());
        _screen.Router.Navigate.Execute(new LoginPageViewModel(_screen));
    }

    private async Task StartGame(CancellationToken cancellationToken)
    {
        try
        {
            var localProfile = new ProfileCreateInfoDto
            {
                ProfileName = ListViewModel.SelectedProfile!.Name,
                RamSize = 8192,
                IsFullScreen = false,
                OsType = ((int)OsType.Windows).ToString(),
                OsArchitecture = Environment.Is64BitOperatingSystem ? "64" : "32",
                UserAccessToken = User.AccessToken,
                UserName = User.Name,
                UserUuid = User.Uuid
            };

            var profileInfo = await _clientManager.GetProfileInfo(localProfile);

            if (profileInfo is { Data: not null })
            {
                await Task.Run(async () => await _clientManager.DownloadNotInstalledFiles(profileInfo.Data), cancellationToken);
                var process = await _clientManager.GetProcess(profileInfo.Data);

                process.Start();
                await process.WaitForExitAsync(cancellationToken);
            }
            else
            {
                if (_screen is MainWindowViewModel mainViewModel)
                {
                    mainViewModel.Manager
                        .CreateMessage(true, "#D03E3E",
                            LocalizationService.GetString(ResourceKeysDictionary.Error),
                            LocalizationService.GetString(ResourceKeysDictionary.ProfileNotConfigured))
                        .Dismiss()
                        .WithDelay(TimeSpan.FromSeconds(3))
                        .Queue();
                }
            }
        }
        catch (Exception exception)
        {
            if (_screen is MainWindowViewModel mainViewModel)
            {
                mainViewModel.Manager
                    .CreateMessage(true, "#D03E3E",
                        LocalizationService.GetString(ResourceKeysDictionary.Error),
                        string.Join(". ", exception.Message))
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(3))
                    .Queue();
            }

            Console.WriteLine(exception);
        }
    }


    private async void LoadData()
    {
        try
        {
            var profilesData = await _clientManager.GetProfiles();

            ListViewModel.Profiles = new ObservableCollection<ProfileReadDto>(profilesData.Data ?? []);
        }
        catch (TaskCanceledException ex)
        {
            await Reconnect();
        }
        catch (HttpRequestException ex)
        {
            await Reconnect();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // ToDo: Send To service
        }
    }

    private async Task Reconnect()
    {
        if (_screen is MainWindowViewModel mainViewModel)
        {
            mainViewModel.Manager
                .CreateMessage()
                .Accent("#F15B19")
                .Background("#111111")
                .HasHeader(LocalizationService.GetString(ResourceKeysDictionary.LostConnection))
                .HasMessage(LocalizationService.GetString(ResourceKeysDictionary.Reconnecting))
                .WithOverlay(new ProgressBar
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 2,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                    Background = Brushes.Transparent,
                    IsIndeterminate = true,
                    IsHitTestVisible = false
                })
                .Dismiss()
                .WithDelay(TimeSpan.FromSeconds(5))
                .Queue();

            await Task.Delay(TimeSpan.FromSeconds(5));

            LoadData();
        }
    }
}
