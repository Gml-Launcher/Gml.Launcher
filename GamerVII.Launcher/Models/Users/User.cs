
namespace GamerVII.Launcher.Models.Users;

public class User : IUser
{
    public required string Login { get; set; }
    public required string Password { get; set; }
    public bool IsLogin { get; set; }
    public string AccessToken { get; set; }
}
