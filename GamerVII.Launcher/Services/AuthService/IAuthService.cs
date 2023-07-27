using System.Threading.Tasks;
using GamerVII.Launcher.Models.Users;

namespace GamerVII.Launcher.Services.AuthService;

public interface IAuthService
{
    Task<IUser> GetAuthorizedUser();
    Task<IUser> OnLogin(string login, string password);
    Task OnLogout();

}