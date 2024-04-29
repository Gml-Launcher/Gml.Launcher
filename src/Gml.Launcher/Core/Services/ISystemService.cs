namespace Gml.Launcher.Core.Services;

public interface ISystemService
{
    string GetApplicationFolder();
    string GetGameFolder(bool needCreate);
}
