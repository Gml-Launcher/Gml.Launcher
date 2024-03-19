namespace Gml.Launcher.Core.Services;

public interface ISystemService
{
    string GetApplicationFolder();
    string GetGameFolder(string additionalPath, bool needCreate);
}
