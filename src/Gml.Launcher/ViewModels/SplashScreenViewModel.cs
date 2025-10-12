using Avalonia.Threading;
using Gml.Client;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using Gml.Web.Api.Domains.System;
using GmlCore.Interfaces.Storage;
using ReactiveUI.Fody.Helpers;
using Sentry;
using Splat;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using IStorageService = Gml.Launcher.Core.Services.IStorageService;

namespace Gml.Launcher.ViewModels;

public class SplashScreenViewModel : WindowViewModelBase
{
    private readonly IStorageService _storageService;
    private readonly ILocalizationService _localizationService;
    private readonly IGmlClientManager _manager;
    private readonly ISystemService _systemService;
    private readonly IBackendChecker _backendChecker;
    private readonly DispatcherTimer _timer;
    private bool _isBackendChecked;
    public IGmlClientManager Manager => _manager;

    public SplashScreenViewModel(
        ISystemService? systemService = null,
        IGmlClientManager? manager = null,
        IStorageService? storage = null,
        ILocalizationService? localizationService = null,
        IBackendChecker? backendChecker = null)
    {
        _storageService = storage ?? Locator.Current.GetService<IStorageService>()
            ?? throw new ServiceNotFoundException(typeof(IStorageService));

        _systemService = systemService ?? Locator.Current.GetService<ISystemService>()
            ?? throw new ServiceNotFoundException(typeof(ISystemService));

        _manager = manager ?? Locator.Current.GetService<IGmlClientManager>()
            ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        _localizationService = localizationService ?? Locator.Current.GetService<ILocalizationService>()
            ?? throw new ServiceNotFoundException(typeof(ILocalizationService));

        _backendChecker = backendChecker ?? Locator.Current.GetService<IBackendChecker>()
            ?? throw new ServiceNotFoundException(typeof(IBackendChecker));

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(15);
        _timer.Tick += OnTimerTick;

        StatusText = _localizationService.GetString(ResourceKeysDictionary.PreparingLaunch);
    }

    [Reactive] public string StatusText { get; set; }
    [Reactive] public bool InfinityLoading { get; set; } = true;
    [Reactive] public short Progress { get; set; }
    [Reactive] public bool IsAuth { get; private set; }
    public ILocalizationService LocalizationService => _localizationService;

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (sender != null && !_isBackendChecked)
        {
            StatusText = _localizationService.GetString(ResourceKeysDictionary.BackendCheckingLonger);
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            var osType = _systemService.GetOsType();
            var osArch = RuntimeInformation.ProcessArchitecture;

            await _systemService.LoadSystemData();

            ChangeState(_localizationService.GetString(ResourceKeysDictionary.BackendChecking), true);
            _isBackendChecked = false;
            _timer.Start();
            await _backendChecker.UpdateBackendStatus();
            _isBackendChecked = true;
            if (_backendChecker.IsOffline)
            {
                ChangeState(_localizationService.GetString(ResourceKeysDictionary.BackendOffline), true);
                await Task.Delay(1000);
            }

            if (!_backendChecker.IsOffline)
            {
                ChangeState(_localizationService.GetString(ResourceKeysDictionary.SentrySDKInit), true);
                await InitializeSentryAsync();
            }

            if (!_manager.SkipUpdate && !_backendChecker.IsOffline)
            {
                ChangeState(_localizationService.GetString(ResourceKeysDictionary.CheckUpdates), true);
                var versionInfo = await CheckActualVersion(osType, osArch);

                if (!versionInfo.IsActuallVersion)
                {
                    ChangeState(_localizationService.GetString(ResourceKeysDictionary.InstallingUpdates), false);

                    var exePath = Process.GetCurrentProcess().MainModule?.FileName
                                  ?? throw new Exception(ResourceKeysDictionary.FailedOs);

                    var process = _manager.ProgressChanged.Subscribe(
                        percentage => Progress = Convert.ToInt16(percentage));

                    await _manager.UpdateCurrentLauncher(versionInfo, osType, Path.GetFileName(exePath));

                    process.Dispose();
                }
            }

            if (!_backendChecker.IsOffline)
            {
                var authUser = await _storageService.GetAsync<AuthUser>(StorageConstants.User);

                IsAuth = authUser != null
                         && authUser.ExpiredDate > DateTime.Now
                         && authUser is { IsAuth: true }
                         && await ValidateToken(authUser)
                         && await ValidateTokenWithApi(authUser);
            }
        }
        catch (Exception exception)
        {
            SentrySdk.CaptureException(exception);
        }
        ChangeState(_localizationService.GetString(ResourceKeysDictionary.Launching), true);
        await Task.Delay(500);
    }

    private async Task<bool> ValidateToken(AuthUser user)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(user.AccessToken))
            return false;

        var jwtToken = handler.ReadJwtToken(user.AccessToken);

        var claims = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

        if (claims?.Value == user.Name)
            return true;

        await _storageService.SetAsync<IUser?>(StorageConstants.User, null).ConfigureAwait(false);

        return false;
    }

    private async Task<bool> ValidateTokenWithApi(AuthUser user)
    {
        var userData = await _manager.Auth(user.AccessToken)
            .ConfigureAwait(false);

        if (userData.User.IsAuth)
            return userData.User.IsAuth;

        await _storageService.SetAsync<IUser?>(StorageConstants.User, null).ConfigureAwait(false);

        return userData.User.IsAuth;
    }

    private async Task<(IVersionFile? ActualVersion, bool IsActuallVersion)> CheckActualVersion(OsType osType,
        Architecture osArch)
    {
        var actualVersion = await _manager.GetActualVersion(osType, osArch);

        if (actualVersion is null) return (null, true);

        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0, 0);

        return actualVersion.Version.Equals(version.ToString())
            ? (actualVersion, true)
            : (actualVersion, false);
    }

    private void ChangeState(string text, bool isInfinity)
    {
        StatusText = text;
        InfinityLoading = isInfinity;
    }

    private static async Task InitializeSentryAsync()
    {
        Debug.WriteLine($"[Gml][{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Start sentry initialization");
        var sentryUrl = await GmlClientManager.GetSentryLinkAsync(ResourceKeysDictionary.Host);

        try
        {
            if (!string.IsNullOrEmpty(sentryUrl))
                await Task.Run(() =>
                {
                    SentrySdk.Init(options =>
                    {
                        options.Dsn = sentryUrl;
#if DEBUG
                    options.Debug = true;
#endif
                        options.TracesSampleRate = 1.0;
                        options.DiagnosticLevel = SentryLevel.Debug;
                        options.IsGlobalModeEnabled = true;
                        options.SendDefaultPii = true;
                        options.MaxAttachmentSize = 10 * 1024 * 1024;
                    });
                });

            Debug.WriteLine($"[Gml][{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Sentry initialized");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}
