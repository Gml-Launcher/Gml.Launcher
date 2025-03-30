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
using Sentry;
using Splat;

namespace Gml.Launcher.ViewModels.Pages;

public class LoginPageViewModel : PageViewModelBase
{
    private readonly IGmlClientManager _gmlClientManager;
    private readonly IObservable<bool> _onClosed;
    private readonly MainWindowViewModel _screen;
    private readonly IStorageService _storageService;
    private readonly ISystemService _systemService;
    private ObservableCollection<string> _errorList = new();
    private bool _isProcessing;
    private string _login = string.Empty;
    private string _password = string.Empty;


    internal LoginPageViewModel(IScreen screen,
        IObservable<bool> onClosed,
        IGmlClientManager? gmlClientManager = null,
        IStorageService? storageService = null,
        ISystemService? systemService = null,
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

        _screen.OnClosed.Subscribe(DisposeConnections);

        LoginCommand = ReactiveCommand.CreateFromTask(OnAuth);

        RxApp.MainThreadScheduler.Schedule(CheckAuth);
    }

    public string Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

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

    public ICommand LoginCommand { get; set; }

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

            var authInfo = await _gmlClientManager.Auth(Login, Password, _systemService.GetHwid());

            if (authInfo.User.IsAuth)
            {
                await _storageService.SetAsync(StorageConstants.User, authInfo.User);
                _screen.Router.Navigate.Execute(new OverviewPageViewModel(_screen, authInfo.User, _onClosed));
                return;
            }

            if (authInfo.Item1.Has2Fa)
                //ToDo: Next versions
                return;

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
