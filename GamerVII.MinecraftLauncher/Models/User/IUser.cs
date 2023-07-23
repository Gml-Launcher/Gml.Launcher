namespace GamerVII.MinecraftLauncher.Models.User;

public interface IUser
{
    string Login { get; set; }
    string Password { get; set; }
    bool IsLogin { get; set; }
}
