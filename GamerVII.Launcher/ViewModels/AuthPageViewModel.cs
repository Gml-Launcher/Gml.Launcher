using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GamerVII.Launcher.Services.AuthService;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.ViewModels;

public class AuthPageViewModel : PageViewModelBase
{
    public delegate void AuthorizeHandler(bool isSuccess);
    public  event AuthorizeHandler Authorized;
    
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
    public bool IsAuthorizing
    {
        get => _isAuthorizing;
        set => this.RaiseAndSetIfChanged(ref _isAuthorizing, value);
    }
    
    
    public ICommand OnLoginCommand { get; }
    
    
    private readonly IAuthService _authService;

    private string _login = string.Empty;
    private string _password = string.Empty;
    private bool _isAuthorizing = false;

    public AuthPageViewModel(IAuthService? authService = null)
    {
        _authService = authService ?? Locator.Current.GetService<IAuthService>()!;
        
        var canLaunch = this.WhenAnyValue(
            x => x.Login, x => x.Password,
            (login, password) =>
                !String.IsNullOrWhiteSpace(login) &&
                !String.IsNullOrWhiteSpace(password)
        );

        OnLoginCommand = ReactiveCommand.Create(OnLogin, canLaunch);
    }

    private async void OnLogin()
    {
        IsAuthorizing = true;
        var user = await _authService.OnLogin(Login, Password);

        if (user != null && user.IsLogin)
        {
            Authorized?.Invoke(true);
        }
        IsAuthorizing = false;

    }
}