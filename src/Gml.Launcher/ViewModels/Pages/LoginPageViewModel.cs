using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GamerVII.Notification.Avalonia;
using Gml.Client;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Sentry;
using Splat;

namespace Gml.Launcher.ViewModels.Pages;

public class LoginPageViewModel : PageViewModelBase
{
    private readonly IGmlClientManager _gmlClientManager;
    private readonly IObservable<bool> _onClosed;
    private readonly MainWindowViewModel _screen;
    private readonly IBackendChecker _backendChecker;
    private readonly IStorageService _storageService;
    private readonly ISystemService _systemService;
    private ObservableCollection<string> _errorList = new();
    private bool _isProcessing;

    internal LoginPageViewModel(IScreen screen,
        IObservable<bool> onClosed,
        IGmlClientManager? gmlClientManager = null,
        IStorageService? storageService = null,
        ISystemService? systemService = null,
        IBackendChecker? backendChecker = null,
        ILocalizationService? localizationService = null) : base(screen, localizationService)
    {
        _screen = (MainWindowViewModel)screen;
        _onClosed = onClosed;

        _storageService = storageService
                          ?? Locator.Current.GetService<IStorageService>()
                          ?? throw new ServiceNotFoundException(typeof(IStorageService));

        _systemService = systemService
                         ?? Locator.Current.GetService<ISystemService>()
                         ?? throw new ServiceNotFoundException(typeof(IStorageService));

        _gmlClientManager = gmlClientManager
                            ?? Locator.Current.GetService<IGmlClientManager>()
                            ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        _backendChecker = backendChecker
                          ?? Locator.Current.GetService<IBackendChecker>()
                          ?? throw new ServiceNotFoundException(typeof(IBackendChecker));


        _screen.OnClosed.Subscribe(DisposeConnections);

        LoginCommand = ReactiveCommand.CreateFromTask(OnAuth);
        Verify2FaCommand = ReactiveCommand.CreateFromTask(OnVerify2Fa);

        RxApp.MainThreadScheduler.Schedule(CheckAuth);
    }

    [Reactive] public string Login { get; set; }
    [Reactive] public string Password { get; set; }
    [Reactive] public string TwoFactorCode { get; set; }
    [Reactive] public bool Is2FaVisible { get; set; }

    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            this.RaiseAndSetIfChanged(ref _isProcessing, value);
            this.RaisePropertyChanged(nameof(IsNotProcessing));
        }
    }

    public ObservableCollection<string> Errors
    {
        get => _errorList;
        set => this.RaiseAndSetIfChanged(ref _errorList, value);
    }

    public bool IsNotProcessing => !_isProcessing;

    public bool BackendIsActive => !_backendChecker.IsOffline;

    public ICommand LoginCommand { get; set; }
    public ICommand Verify2FaCommand { get; set; }

    private void DisposeConnections(bool isClosed)
    {
        _gmlClientManager.Dispose();
    }

    private async void CheckAuth()
    {
        var authUser = await _storageService.GetAsync<AuthUser>(StorageConstants.User);

        if (authUser is { IsAuth: true } && authUser.ExpiredDate > DateTime.Now)
        {
            _screen.Router.Navigate.Execute(new OverviewPageViewModel(_screen, authUser, _onClosed));
            await _gmlClientManager.OpenServerConnection(authUser);
        }
    }

    private async Task OnAuth(CancellationToken arg)
    {
        try
        {
            IsProcessing = true;
            Errors.Clear();

            Debug.WriteLine("Starting authentication...");
            var authInfo = await _gmlClientManager.Auth(Login, Password, _systemService.GetHwid());
            Debug.WriteLine($"Auth response - IsAuth: {authInfo.User?.IsAuth}, Has2Fa: {authInfo.User?.Has2Fa}");
            Debug.WriteLine($"Auth message: {authInfo.Message}");
            Debug.WriteLine($"Auth details: {string.Join(", ", authInfo.Details)}");

            // Проверяем, требуется ли 2FA по сообщению об ошибке
            if (authInfo.Message?.Contains("2FA") == true ||
                authInfo.Details?.Any(d => d.Contains("2FA")) == true)
            {
                Debug.WriteLine("2FA required based on error message");
                Is2FaVisible = true;
                TwoFactorCode = string.Empty;
                return;
            }

            if (authInfo.User?.IsAuth == true)
            {
                if (authInfo.User.Has2Fa)
                {
                    Debug.WriteLine("User has 2FA enabled, showing 2FA input");
                    Is2FaVisible = true;
                    TwoFactorCode = string.Empty;
                    return;
                }

                Debug.WriteLine("Authentication successful, no 2FA required");
                await _storageService.SetAsync(StorageConstants.User, authInfo.User);
                _screen.Router.Navigate.Execute(new OverviewPageViewModel(_screen, authInfo.User, _onClosed));
                return;
            }

            Debug.WriteLine("Authentication failed");
            if (_screen is { } mainView)
            {
                if (!authInfo.Details.Any())
                    mainView.Manager
                        .CreateMessage(true, "#D03E3E",
                            LocalizationService.GetString(ResourceKeysDictionary.InvalidAuthData),
                            authInfo.Message)
                        .Dismiss()
                        .WithDelay(TimeSpan.FromSeconds(3))
                        .Queue();

                Errors = new ObservableCollection<string>(authInfo.Details);
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Authentication error: {exception}");
            if (_screen is { } mainView)
            {
                mainView.Manager
                    .CreateMessage(true, "#D03E3E",
                        LocalizationService.GetString(ResourceKeysDictionary.InvalidAuthData),
                        exception.Message)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(3))
                    .Queue();
            }

            Debug.WriteLine(exception);
            SentrySdk.CaptureException(exception);
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task OnVerify2Fa(CancellationToken arg)
    {
        try
        {
            IsProcessing = true;
            Errors.Clear();

            Debug.WriteLine($"Verifying 2FA code: {TwoFactorCode}");
            var authInfo = await _gmlClientManager.Auth2Fa(Login, Password, _systemService.GetHwid(), TwoFactorCode);
            Debug.WriteLine($"2FA verification response - IsAuth: {authInfo.User?.IsAuth}, Has2Fa: {authInfo.User?.Has2Fa}");
            Debug.WriteLine($"2FA verification message: {authInfo.Message}");
            Debug.WriteLine($"2FA verification details: {string.Join(", ", authInfo.Details)}");

            if (authInfo.User?.IsAuth == true)
            {
                Debug.WriteLine("2FA verification successful");
                await _storageService.SetAsync(StorageConstants.User, authInfo.User);
                Is2FaVisible = false; // Скрываем окно 2FA
                _screen.Router.Navigate.Execute(new OverviewPageViewModel(_screen, authInfo.User, _onClosed));
                return;
            }

            Debug.WriteLine("2FA verification failed");
            if (_screen is { } mainView)
            {
                // Если код неверный, показываем ошибку, но оставляем окно 2FA открытым
                mainView.Manager
                    .CreateMessage(true, "#D03E3E",
                        LocalizationService.GetString(ResourceKeysDictionary.InvalidAuthData),
                        authInfo.Message ?? "Invalid 2FA code")
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(3))
                    .Queue();

                if (authInfo.Details.Any())
                {
                    Errors = new ObservableCollection<string>(authInfo.Details);
                }

                // Очищаем поле для ввода кода
                TwoFactorCode = string.Empty;
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"2FA verification error: {exception}");
            if (_screen is { } mainView)
            {
                mainView.Manager
                    .CreateMessage(true, "#D03E3E",
                        LocalizationService.GetString(ResourceKeysDictionary.InvalidAuthData),
                        exception.Message)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(3))
                    .Queue();
            }

            Debug.WriteLine(exception);
            SentrySdk.CaptureException(exception);
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
