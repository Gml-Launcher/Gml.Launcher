using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using GamerVII.Notification.Avalonia;
using Gml.Client;
using Gml.Client.Helpers;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.Models;
using Gml.Launcher.ViewModels.Base;
using Gml.Launcher.ViewModels.Components;
using Gml.Web.Api.Dto.Messages;
using Gml.Web.Api.Dto.News;
using Gml.Web.Api.Dto.Profile;
using GmlCore.Interfaces.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Sentry;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gml.Launcher.ViewModels.Pages;

public class OverviewPageViewModel : PageViewModelBase
{
    private readonly IDisposable? _closeEvent;
    private readonly IGmlClientManager _gmlManager;
    private readonly IDisposable? _loadedLoaded;
    private readonly MainWindowViewModel _mainViewModel;
    private readonly IDisposable? _maxCountLoaded;
    private readonly IObservable<bool> _onClosed;
    private readonly LogHandler _logHandler;
    private readonly IDisposable _profileNameChanged;
    private readonly IStorageService _storageService;
    private readonly ISystemService _systemService;
    private readonly IBackendChecker _backendChecker;
    private Process? _gameProcess;
    private readonly ISettingsService _settingsService;
    private readonly IDisposable? _speedSubscription;

    internal OverviewPageViewModel(IScreen screen,
        IUser user,
        IObservable<bool> onClosed,
        IGmlClientManager? gmlManager = null,
        ISystemService? systemService = null,
        IStorageService? storageService = null,
        ISettingsService? settingsService = null,
        IBackendChecker? backendChecker = null,
        LogHandler? logHandler = null) : base(screen)
    {
        _mainViewModel = screen as MainWindowViewModel ?? throw new Exception("Not valid screen");
        User = user;
        _onClosed = onClosed;

        _logHandler = logHandler
                      ?? Locator.Current.GetService<LogHandler>()
                      ?? throw new ServiceNotFoundException(typeof(LogHandler));

        _systemService = systemService
                         ?? Locator.Current.GetService<ISystemService>()
                         ?? throw new ServiceNotFoundException(typeof(ISystemService));

        _storageService = storageService
                          ?? Locator.Current.GetService<IStorageService>()
                          ?? throw new ServiceNotFoundException(typeof(IStorageService));

        _gmlManager = gmlManager
                      ?? Locator.Current.GetService<IGmlClientManager>()
                      ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        _backendChecker = backendChecker
                          ?? Locator.Current.GetService<IBackendChecker>()
                          ?? throw new ServiceNotFoundException(typeof(IBackendChecker));

        _settingsService = settingsService
                           ?? Locator.Current.GetService<ISettingsService>()
                           ?? throw new ServiceNotFoundException(typeof(ISettingsService));

        GoProfileCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new ProfilePageViewModel(screen, User, _gmlManager))
        );

        GoModsCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new ModsPageViewModel(
                screen,
                ListViewModel.SelectedProfile!,
                User,
                _gmlManager,
                _systemService))
        );

        GoSettingsCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new SettingsPageViewModel(
                screen,
                LocalizationService,
                _settingsService,
                _gmlManager))
        );

        HomeCommand = ReactiveCommand.Create(async () => await LoadProfiles());

        _gmlManager.ProgressChanged.Subscribe(percentage =>
        {
            if (LoadingPercentage != percentage)
                LoadingPercentage = percentage;
        });

        _gmlManager.ProfilesChanges.Subscribe(LoadProfilesAsync);

        _closeEvent ??= onClosed.Subscribe(KillGameProcess);
        _profileNameChanged ??= ListViewModel.ProfileChanged.Subscribe(OnProfileChanged);

        _maxCountLoaded ??= _gmlManager.MaxFileCount.Subscribe(count => MaxCount = count);
        _loadedLoaded ??= _gmlManager.LoadedFilesCount.Subscribe(ChangeLoadProcessDescription);

        _speedSubscription = _gmlManager.DownloadedBytesDelta
            .Buffer(TimeSpan.FromSeconds(1))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(list =>
            {
                var total = list.Sum(x => (long)x);
                LoadingSpeed = total > 0 ? $"{FormatBytes(total)}/s" : string.Empty;
            });

        LogoutCommand = ReactiveCommand.CreateFromTask(OnLogout);

        PlayCommand = ReactiveCommand.CreateFromTask(StartGame);

        BackendIsActive = !_backendChecker.IsOffline;

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }
    [Reactive] public bool IsModsButtonVisible { get; private set; }

    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public ICommand GoProfileCommand { get; set; }
    public ICommand GoModsCommand { get; set; }
    public ICommand LogoutCommand { get; set; }
    public ICommand PlayCommand { get; set; }
    public ICommand GoSettingsCommand { get; set; }
    public ICommand HomeCommand { get; set; }
    public ListViewModel ListViewModel { get; } = new();
    public IUser User { get; }
    public bool BackendIsActive { get; }

    [Reactive] public int? LoadingPercentage { get; set; }
    [Reactive] public int? MaxCount { get; set; }
    [Reactive] public int? LoadedCount { get; set; }

    [Reactive] public string? Headline { get; set; }

    [Reactive] public string? Description { get; set; }

    [Reactive] public string? LoadingSpeed { get; set; }

    [Reactive] public bool IsProcessing { get; set; }

    [Reactive] public ObservableCollection<NewsReadDto> News { get; set; } = [];

    private async void LoadProfilesAsync(bool eventInfo)
    {
        await LoadProfiles();
    }

    private void OnProfileChanged(ProfileReadDto? profile)
    {
        SaveSelectedServer(profile);
        UpdateModsButtonVisibility(profile);
    }
    private void UpdateModsButtonVisibility(ProfileReadDto? profile)
    {
        if (profile is null)
        {
            IsModsButtonVisible = false;
            return;
        }

        var keywords = new[]
        {
            nameof(GameLoader.Forge),
            nameof(GameLoader.Fabric),
            nameof(GameLoader.Quilt),
            nameof(GameLoader.NeoForge)
        };

        var profileName = profile.Name.ToLowerInvariant();
        var gameVersion = profile.GameVersion?.ToLowerInvariant() ?? string.Empty;
        var launchVersion = profile.LaunchVersion?.ToLowerInvariant() ?? string.Empty;

        IsModsButtonVisible = keywords.Any(keyword =>
            profileName.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
            gameVersion.Contains(keyword, StringComparison.CurrentCultureIgnoreCase) ||
            launchVersion.Contains(keyword, StringComparison.CurrentCultureIgnoreCase));
    }

    private void ChangeLoadProcessDescription(int count)
    {
        LoadedCount = count;

        Description =
            $"{LocalizationService.GetString(ResourceKeysDictionary.Stay)}: " +
            $"{MaxCount - LoadedCount} " +
            $"{LocalizationService.GetString(ResourceKeysDictionary.Files)}";
    }

    private async void SaveSelectedServer(ProfileReadDto? profile)
    {
        if (profile != null)
            await _storageService.SetAsync(StorageConstants.LastSelectedProfileName, profile.Name);
    }

    private Task OnLogout(CancellationToken arg)
    {
        return Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await _storageService.SetAsync<IUser?>(StorageConstants.User, null);
            _mainViewModel.Router.Navigate.Execute(new LoginPageViewModel(_mainViewModel, _onClosed));
        });
    }

    private async Task StartGame()
    {
        var tokenSource = new CancellationTokenSource();
        var cancellationToken = tokenSource.Token;

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
                    _gameProcess.StartWatch();
                    _gameProcess.BeginOutputReadLine();
                    _gameProcess.BeginErrorReadLine();

                    Dispatcher.UIThread.Invoke(() => _mainViewModel._gameLaunched.OnNext(true));
                    UpdateProgress(string.Empty, string.Empty, false);
                    await _gameProcess.WaitForExitAsync(cancellationToken);

                    if (_gameProcess.ExitCode != 0)
                    {
                        throw new MinecraftException(
                            $"Произошел краш игры. {string.Join("\n", _logHandler.RecentLogs)}");
                    }
                }
                else
                {
                    ShowError(ResourceKeysDictionary.Error, ResourceKeysDictionary.ProfileNotConfigured);
                }
            }
            catch (UnauthorizedAccessException)
            {
                await OnLogout(CancellationToken.None);
            }
            catch (FileNotFoundException exception)
            {
                ShowError(ResourceKeysDictionary.Error,
                    LocalizationService.GetString(ResourceKeysDictionary.JavaNotFound));
                SentrySdk.CaptureException(exception);
                Console.WriteLine(exception);
            }
            catch (IOException ioException) when (_systemService.IsDiskFull(ioException))
            {
                ShowError(ResourceKeysDictionary.Error,
                    LocalizationService.GetString(ResourceKeysDictionary.IsDiskFull));

                SentrySdk.CaptureException(ioException);
                Console.WriteLine(ioException);
            }
            catch (MinecraftException exception)
            {
                _logHandler.RecentLogs.Clear();
                ShowError(ResourceKeysDictionary.Error,
                    LocalizationService.GetString(SystemConstants.MinecraftExceptionStartException));

                SentrySdk.CaptureException(exception);
            }
            catch (TaskCanceledException exception)
            {
                ShowError(ResourceKeysDictionary.Error,
                    LocalizationService.GetString(SystemConstants.MinecraftExceptionStartException));

                SentrySdk.CaptureException(exception);
                Console.WriteLine(exception);
                _logHandler.RecentLogs.Clear();
            }
            catch (Exception exception)
            {
                ShowError(ResourceKeysDictionary.Error, string.Join(". ", exception.Message));

                SentrySdk.CaptureException(exception);
                Console.WriteLine(exception);
            }
            finally
            {
                _gameProcess?.Dispose();
                Dispatcher.UIThread.Invoke(() => _mainViewModel._gameLaunched.OnNext(false));
                UpdateProgress(string.Empty, string.Empty, false);
                if (!_backendChecker.IsOffline)
                {
                    await _gmlManager.UpdateDiscordRpcState(
                        LocalizationService.GetString(ResourceKeysDictionary.DefaultDRpcText));
                }
            }
        });
    }

    private async Task<Process> GenerateProcess(CancellationToken cancellationToken,
        ResponseMessage<ProfileReadInfoDto?> profileInfo)
    {
        UpdateProgress(
            LocalizationService.GetString(ResourceKeysDictionary.Updating),
            LocalizationService.GetString(ResourceKeysDictionary.CheckingFileIntegrity),
            true);

        if (profileInfo.Data is null)
            throw new Exception(LocalizationService.GetString(ResourceKeysDictionary.ProfileNotConfigured));

        if (!_backendChecker.IsOffline)
        {
            await _gmlManager.DownloadNotInstalledFiles(profileInfo.Data, cancellationToken);
        }

        var process = await _gmlManager.GetProcess(profileInfo.Data, _systemService.GetOsType(), _backendChecker.IsOffline);

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine(e.Data);
                _logHandler.ProcessLogs(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data is null || string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            Debug.WriteLine(e.Data);

            _logHandler.ProcessLogs(e.Data);
        };

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

        if (!_backendChecker.IsOffline)
        {
            await _gmlManager.UpdateDiscordRpcState(
                $"{LocalizationService.GetString(ResourceKeysDictionary.PlayDRpcText)} \"{ListViewModel.SelectedProfile!.Name}\"");
        }

        var settings = await _storageService.GetAsync<SettingsInfo>(StorageConstants.Settings) ?? SettingsInfo.Default;

        var localProfile = new ProfileCreateInfoDto
        {
            ProfileName = ListViewModel.SelectedProfile!.Name,
            RamSize = CalculateRecommendedRam(settings),
            IsFullScreen = settings.FullScreen,
            OsType = ((int)_systemService.GetOsType()).ToString(),
            OsArchitecture = _systemService.GetOsArchitecture(),
            UserAccessToken = User.AccessToken,
            UserName = User.Name,
            UserUuid = User.Uuid,
            WindowWidth = settings.GameWidth,
            WindowHeight = settings.GameHeight
        };

        ResponseMessage<ProfileReadInfoDto?>? profileInfo;

        if (_backendChecker.IsOffline)
        {
            profileInfo = await _gmlManager.GetProfileInfoOffline(localProfile);
        }
        else
        {
            profileInfo = await _gmlManager.GetProfileInfo(localProfile);
        }

        return profileInfo;
    }

    private int CalculateRecommendedRam(SettingsInfo settings)
    {
        var maxRam = (int)_systemService.GetMaxRam();
        var recommendedRam = settings.IsDynamicRam ? ListViewModel.SelectedProfile?.RecommendedRam ?? 1024 : Convert.ToInt32(settings.RamValue);

        var minValue = Math.Min(recommendedRam, maxRam);

        return minValue == 0 && ListViewModel.SelectedProfile?.RecommendedRam == 0 ? 512 : minValue;
    }

    private static string FormatBytes(long bytes)
    {
        const int scale = 1024;
        string[] orders = ["B", "KB", "MB", "GB", "TB"];
        double max = bytes;
        int order = 0;
        while (max >= scale && order < orders.Length - 1)
        {
            order++;
            max /= scale;
        }
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.##} {1}", max, orders[order]);
    }

    private void UpdateProgress(string headline, string description, bool isProcessing, int? percentage = null)
    {

        Dispatcher.UIThread.Invoke(() =>
        {
            Headline = headline;
            Description = description;
            IsProcessing = isProcessing;
            LoadingPercentage = percentage;
            if (!IsProcessing)
                LoadingSpeed = string.Empty;
        });
    }

    private async void KillGameProcess(bool isClosed)
    {
        try
        {
            _gameProcess?.Kill();
        }
        catch (Exception exception)
        {
            SentrySdk.CaptureException(exception);
        }
    }

    private async void LoadData()
    {
        try
        {
            await LoadProfiles();
            await LoadNews();

            if (!_backendChecker.IsOffline)
            {
                await _gmlManager.LoadDiscordRpc();
                await _gmlManager.UpdateDiscordRpcState(
                    LocalizationService.GetString(ResourceKeysDictionary.DefaultDRpcText));
            }
        }
        catch (TaskCanceledException exception)
        {
            SentrySdk.CaptureException(exception);
            Console.WriteLine(exception);
            await Reconnect();
        }
        catch (HttpRequestException exception)
        {
            SentrySdk.CaptureException(exception);
            Console.WriteLine(exception);
            await Reconnect();
        }
        catch (Exception exception)
        {
            SentrySdk.CaptureException(exception);
        }
    }

    private async Task LoadNews()
    {
        try
        {
            if (_backendChecker.IsOffline)
            {
                News =
                [
                    new NewsReadDto
                    {
                        Title = LocalizationService.GetString(ResourceKeysDictionary.NewsOffline),
                        Content =
                            $"<div style='text-align: center; margin-top: 100 px; margin-bottom: 100 px;'>{LocalizationService.GetString(ResourceKeysDictionary.NewsOffline)}</div>",
                        Date = null,
                        Type = NewsListenerType.Custom
                    }
                ];
                return;
            }

            var news = await _gmlManager.GetNews();

            if (news.Data?.Count == 0)
            {
                News =
                [
                    new NewsReadDto
                    {
                        Title = LocalizationService.GetString(ResourceKeysDictionary.NewsEmptyTitle),
                        Content =
                            $"<div style='text-align: center; margin-top: 100 px; margin-bottom: 100 px;'>{LocalizationService.GetString(ResourceKeysDictionary.NewsEmptyContent)}</div>",
                        Date = null,
                        Type = NewsListenerType.Custom
                    }
                ];
            }
            else
            {
                News = new ObservableCollection<NewsReadDto>(news.Data ?? []);
            }
            ;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task LoadProfiles()
    {
        ResponseMessage<List<ProfileReadDto>> profilesData;
        if (_backendChecker.IsOffline)
        {
            profilesData = await _gmlManager.GetOfflineProfiles();
        }
        else
        {
            profilesData = await _gmlManager.GetProfiles(User.AccessToken);
        }

        ListViewModel.Profiles = new ObservableCollection<ProfileReadDto>(profilesData.Data ?? []);

        var lastSelectedProfileName = await _storageService.GetAsync<string>(StorageConstants.LastSelectedProfileName);

        if (!string.IsNullOrEmpty(lastSelectedProfileName) &&
            ListViewModel.Profiles.FirstOrDefault(c => c.Name == lastSelectedProfileName) is { } profile)
            ListViewModel.SelectedProfile = profile;

        ListViewModel.SelectedProfile ??= ListViewModel.Profiles.FirstOrDefault();

        // НОВОЕ: Устанавливаем начальное состояние видимости кнопки при первой загрузке
        UpdateModsButtonVisibility(ListViewModel.SelectedProfile);
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
