using System.Threading.Tasks;
using GamerVII.Launcher.Models.Users;

namespace GamerVII.Launcher.Services.AuthService;

/// <summary>
/// Represents the service for managing user authentication.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Retrieves the authorized user.
    /// </summary>
    /// <returns>An instance of IUser representing the authorized user.</returns>
    Task<IUser> GetAuthorizedUser();

    /// <summary>
    /// Performs login operation using the provided login credentials.
    /// </summary>
    /// <param name="login">The user's login.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>An instance of IUser representing the authenticated user.</returns>
    Task<IUser> OnLogin(string login, string password);

    /// <summary>
    /// Performs logout operation for the currently authenticated user.
    /// </summary>
    Task OnLogout();
}
