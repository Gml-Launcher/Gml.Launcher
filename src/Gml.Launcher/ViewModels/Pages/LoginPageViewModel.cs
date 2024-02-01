using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using GamerVII.Notification.Avalonia;
using Gml.Client;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace Gml.Launcher.ViewModels.Pages;

public class LoginPageViewModel : PageViewModelBase
{
    private string _login = string.Empty;
    private bool _isProcessing = false;
    private string _password = string.Empty;
    private readonly IScreen _screen;
    private readonly IStorageService _storageService;
    private readonly IGmlClientManager _gmlClientManager;

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

    public bool IsNotProcessing => !_isProcessing;

    public ICommand LoginCommand { get; set; }


    internal LoginPageViewModel(
        IScreen screen,
        IGmlClientManager? gmlClientManager = null,
        IStorageService? storageService = null,
        ILocalizationService? localizationService = null) : base(screen, localizationService)
    {
        _screen = screen;

        _storageService = storageService
                          ?? Locator.Current.GetService<IStorageService>()
                          ?? throw new ServiceNotFoundException(typeof(IStorageService));

        _gmlClientManager = gmlClientManager
                            ?? Locator.Current.GetService<IGmlClientManager>()
                            ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));


        LoginCommand = ReactiveCommand.CreateFromTask(OnAuth);

        RxApp.MainThreadScheduler.Schedule(CheckAuth);
    }

    private async void CheckAuth()
    {
        var authUser = await _storageService.GetAsync<AuthUser>(StorageConstants.User);

        if (authUser is { IsAuth: true })
            _screen.Router.Navigate.Execute(new OverviewPageViewModel(_screen));
    }

    private async Task OnAuth(CancellationToken arg)
    {
        try
        {
            // this.Manager
            //     .CreateMessage()
            //     .Accent("#1751C3")
            //     .Animates(true)
            //     .Background("#333")
            //     .HasBadge("Info")
            //     .HasMessage("Update will be installed on next application restart. This message will be dismissed after 5 seconds.")
            //     .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
            //     .Queue();

            IsProcessing = true;
            var user = await _gmlClientManager.Auth(Login, Password);

            if (user.Item1.IsAuth)
            {
                await _storageService.SetAsync(StorageConstants.User, user.Item1);
                _screen.Router.Navigate.Execute(new OverviewPageViewModel(_screen));
                return;
            }

            if (user.Item1.Has2Fa)
            {
                //ToDo: Next versions

                return;
            }

            if (_screen is MainWindowViewModel mainView)
            {
                mainView.Manager
                    .CreateMessage(true, "#D03E3E",
                        LocalizationService.GetString(ResourceKeysDictionary.Error),
                        string.Join(". ",
                            LocalizationService.GetString(ResourceKeysDictionary.InvalidAuthData),
                            user.Item2))
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(3))
                    .Queue();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
