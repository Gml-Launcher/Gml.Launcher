using GamerVII.MinecraftLauncher.Models.User;
using System.Threading.Tasks;

namespace GamerVII.MinecraftLauncher.Services.Auth;

public interface IAuthService
{

    Task<IUser> OnLogin(string login, string password);

}
