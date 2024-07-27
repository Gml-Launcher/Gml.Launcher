using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using GamerVII.Notification.Avalonia;
using Gml.Client;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.Models;
using Gml.Launcher.ViewModels.Base;
using Gml.Launcher.ViewModels.Components;
using Gml.Web.Api.Dto.Messages;
using Gml.Web.Api.Dto.Profile;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Gml.Launcher.ViewModels.Pages;

public class OverviewPageViewModel : PageViewModelBase
{
    private readonly IStorageService _storageService;
    private readonly IGmlClientManager _gmlManager;
    private readonly IUser _user;
    private readonly IObservable<bool> _onClosed;
    private readonly ISystemService _systemService;
    private readonly IDisposable? _closeEvent;
    private readonly IDisposable? _maxCountLoaded;
    private readonly IDisposable? _loadedLoaded;
    private readonly IDisposable _profileNameChanged;
    private Process? _gameProcess;
    private readonly MainWindowViewModel _mainViewModel;
    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public ICommand GoProfileCommand { get; set; }
    public ICommand LogoutCommand { get; set; }
    public ICommand PlayCommand { get; set; }
    public ICommand GoSettingsCommand { get; set; }
    public ICommand HomeCommand { get; set; }
    public ListViewModel ListViewModel { get; } = new();
    public IUser User => _user;

    [Reactive] public int? LoadingPercentage { get; set; }
    [Reactive] public int? MaxCount { get; set; }
    [Reactive] public int? LoadedCount { get; set; }

    [Reactive] public string? Headline { get; set; }

    [Reactive] public string? Description { get; set; }

    [Reactive] public bool IsProcessing { get; set; }

    internal OverviewPageViewModel(IScreen screen,
        IUser user,
        IObservable<bool> onClosed,
        IGmlClientManager? gmlManager = null,
        ISystemService? systemService = null,
        IStorageService? storageService = null) : base(screen)
    {
        _mainViewModel = screen as MainWindowViewModel ?? throw new Exception("Not valid screen");
        _user = user;
        _onClosed = onClosed;
        _systemService = systemService
                         ?? Locator.Current.GetService<ISystemService>()
                         ?? throw new ServiceNotFoundException(typeof(ISystemService));

        _storageService = storageService
                          ?? Locator.Current.GetService<IStorageService>()
                          ?? throw new ServiceNotFoundException(typeof(IStorageService));

        _gmlManager = gmlManager
                         ?? Locator.Current.GetService<IGmlClientManager>()
                         ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        GoProfileCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new ProfilePageViewModel(screen, _user, gmlManager))
        );

        GoSettingsCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new SettingsPageViewModel(
                screen,
                LocalizationService,
                _storageService,
                _systemService,
                _gmlManager,
                ListViewModel.SelectedProfile!))
        );

        HomeCommand = ReactiveCommand.Create(async () => await LoadProfiles());

        _gmlManager.ProgressChanged.Subscribe(percentage =>
        {
            if (LoadingPercentage != percentage)
                LoadingPercentage = percentage;
        });

        _gmlManager.ProfilesChanges.Subscribe(async eventInfo => await LoadProfiles());

        _closeEvent ??= onClosed.Subscribe(_ => _gameProcess?.Kill());
        _profileNameChanged ??= ListViewModel.ProfileChanged.Subscribe(SaveSelectedServer);
        _maxCountLoaded ??= _gmlManager.MaxFileCount.Subscribe(count => MaxCount = count);
        _loadedLoaded ??= _gmlManager.LoadedFilesCount.Subscribe(ChangeLoadProcessDescription);

        LogoutCommand = ReactiveCommand.CreateFromTask(OnLogout);

        PlayCommand = ReactiveCommand.CreateFromTask(StartGame);

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }

    private void ChangeLoadProcessDescription(int count)
    {
        LoadedCount = count;

        Description = $"{LocalizationService.GetString(ResourceKeysDictionary.Stay)}: {MaxCount - LoadedCount}";
    }

    private async void SaveSelectedServer(ProfileReadDto? profile)
    {
        if (profile != null)
            await _storageService.SetAsync(StorageConstants.LastSelectedProfileName, profile.Name);
    }

    private async Task OnLogout(CancellationToken arg)
    {
        await _storageService.SetAsync(StorageConstants.User, new AuthUser());
        _mainViewModel.Router.Navigate.Execute(new LoginPageViewModel(_mainViewModel, _onClosed));
    }

    private async Task StartGame(CancellationToken cancellationToken)
    {

        await ExecuteFromNewThread(async () =>
        {
            try
            {
                var profileInfo = await GetProfileInfo();

                if (profileInfo is { Data: not null })
                {
                    _gameProcess?.Close();
                    _gameProcess = await GenerateProcess(cancellationToken, profileInfo);
                    _gameProcess.Start();
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    Dispatcher.UIThread.Invoke(() => _mainViewModel.gameLaunched.OnNext(true));
                    UpdateProgress(string.Empty, string.Empty, false);
                    await _gameProcess.WaitForExitAsync(cancellationToken);
                }
                else
                {
                    ShowError(ResourceKeysDictionary.Error, ResourceKeysDictionary.ProfileNotConfigured);
                }
            }
            catch (FileNotFoundException exception)
            {
                ShowError(ResourceKeysDictionary.Error, LocalizationService.GetString(ResourceKeysDictionary.JavaNotFound));

                Console.WriteLine(exception);
            }
            catch (Exception exception)
            {
                ShowError(ResourceKeysDictionary.Error, string.Join(". ", exception.Message));

                Console.WriteLine(exception);
            }
            finally
            {
                Dispatcher.UIThread.Invoke(() => _mainViewModel.gameLaunched.OnNext(false));
                UpdateProgress(string.Empty, string.Empty, false);
                await _gmlManager.UpdateDiscordRpcState(LocalizationService.GetString(ResourceKeysDictionary.DefaultDRpcText));
            }
        });



    }

    private async Task<Process> GenerateProcess(CancellationToken cancellationToken, ResponseMessage<ProfileReadInfoDto?> profileInfo)
    {
        UpdateProgress(
            LocalizationService.GetString(ResourceKeysDictionary.Updating),
            LocalizationService.GetString(ResourceKeysDictionary.CheckingFileIntegrity),
            true);

        if (profileInfo.Data is null)
        {
            throw new Exception(LocalizationService.GetString(ResourceKeysDictionary.ProfileNotConfigured));
        }

        await _gmlManager.DownloadNotInstalledFiles(profileInfo.Data, cancellationToken);

        var process = await _gmlManager.GetProcess(profileInfo.Data, _systemService.GetOsType());

        await _gmlManager.ClearFiles(profileInfo.Data);

        UpdateProgress(
            LocalizationService.GetString(ResourceKeysDictionary.Launching),
            LocalizationService.GetString(ResourceKeysDictionary.PreparingLaunch),
            true);

        return process;
    }

    private async Task<ResponseMessage<ProfileReadInfoDto?>?> GetProfileInfo()
    {
        UpdateProgress(
            LocalizationService.GetString(ResourceKeysDictionary.Updating),
            LocalizationService.GetString(ResourceKeysDictionary.UpdatingDescription),
            true);

        await _gmlManager.UpdateDiscordRpcState($"{LocalizationService.GetString(ResourceKeysDictionary.PlayDRpcText)} \"{ListViewModel.SelectedProfile!.Name}\"");

        var settings = await _storageService.GetAsync<SettingsInfo>(StorageConstants.Settings) ?? SettingsInfo.Default;

        var localProfile = new ProfileCreateInfoDto
        {
            ProfileName = ListViewModel.SelectedProfile!.Name,
            RamSize = Convert.ToInt32(settings.RamValue),
            IsFullScreen = settings.FullScreen,
            OsType = ((int)_systemService.GetOsType()).ToString(),
            OsArchitecture = Environment.Is64BitOperatingSystem ? "64" : "32",
            UserAccessToken = User.AccessToken,
            UserName = User.Name,
            UserUuid = User.Uuid,
            WindowWidth = settings.GameWidth,
            WindowHeight = settings.GameHeight
        };

        var profileInfo = await _gmlManager.GetProfileInfo(localProfile);
        return profileInfo;
    }

    private void UpdateProgress(string headline, string description, bool isProcessing, int? percentage = null)
    {
        Headline = headline;
        Description = description;
        IsProcessing = isProcessing;
        LoadingPercentage = percentage;
    }

    private async void LoadData()
    {
        try
        {
            await LoadProfiles();

            await _gmlManager.LoadDiscordRpc();
            await _gmlManager.UpdateDiscordRpcState(LocalizationService.GetString(ResourceKeysDictionary.DefaultDRpcText));

        }
        catch (TaskCanceledException exception)
        {
            Console.WriteLine(exception);
            await Reconnect();
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine(exception);
            await Reconnect();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // ToDo: Send To service
        }
    }

    private async Task LoadProfiles()
    {
        var profilesData = await _gmlManager.GetProfiles();

        ListViewModel.Profiles = new ObservableCollection<ProfileReadDto>(profilesData.Data ?? []);

        var lastSelectedProfileName = await _storageService.GetAsync<string>(StorageConstants.LastSelectedProfileName);

        if (!string.IsNullOrEmpty(lastSelectedProfileName) &&
            ListViewModel.Profiles.FirstOrDefault(c => c.Name == lastSelectedProfileName) is { } profile)
        {
            ListViewModel.SelectedProfile = profile;
        }

        ListViewModel.SelectedProfile ??= ListViewModel.Profiles.FirstOrDefault();

    }

    private async Task Reconnect()
    {
        _mainViewModel.Manager
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
