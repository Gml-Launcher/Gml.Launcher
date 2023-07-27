using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GamerVII.Launcher.Services.AuthService;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;

namespace GamerVII.Launcher.ViewModels;

/// <summary>
/// View model class for the authentication page, derived from PageViewModelBase.
/// </summary>
public class AuthPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Delegate for handling the authorization event.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the authorization was successful.</param>
    public delegate void AuthorizeHandler(bool isSuccess);

    /// <summary>
    /// Event raised when the authorization process is completed.
    /// </summary>
    public event AuthorizeHandler Authorized;

    /// <summary>
    /// Gets or sets the login string for authentication.
    /// </summary>
    public string Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    /// <summary>
    /// Gets or sets the password string for authentication.
    /// </summary>
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user is currently being authorized.
    /// </summary>
    public bool IsAuthorizing
    {
        get => _isAuthorizing;
        set => this.RaiseAndSetIfChanged(ref _isAuthorizing, value);
    }

    /// <summary>
    /// Command that triggers the login process.
    /// </summary>
    public ICommand OnLoginCommand { get; }

    private readonly IAuthService _authService;
    private string _login = string.Empty;
    private string _password = string.Empty;
    private bool _isAuthorizing = false;

    /// <summary>
    /// Initializes a new instance of the AuthPageViewModel class.
    /// </summary>
    /// <param name="authService">An optional IAuthService implementation for user authentication.</param>
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
