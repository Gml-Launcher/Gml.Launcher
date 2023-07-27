namespace GamerVII.Launcher.Models.Users;
public interface IUser
{
    string Login { get; set; }
    string Password { get; set; }
    bool IsLogin { get; set; }
    string AccessToken { get; set; }
}
