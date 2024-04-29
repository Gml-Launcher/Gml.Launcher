using System;
using System.IO;
using Avalonia.Controls.Shapes;
using static System.OperatingSystem;

namespace Gml.Launcher.Core.Services;

public class SystemService : ISystemService
{
    private const string NotSupportedMessage = "Operating system not supported";

    public string GetApplicationFolder()
    {
        if (IsWindows())
        {
            return GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        if (IsLinux() || IsMacOS())
        {
            return GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        throw new NotSupportedException(NotSupportedMessage);
    }

    public string GetGameFolder(bool needCreate)
    {
        var directoryInfo = new DirectoryInfo(System.IO.Path.Combine(GetApplicationFolder(), "GamerVII")); // ToDo: To const

        if (needCreate && !directoryInfo.Exists)
            directoryInfo.Create();

        return directoryInfo.FullName;
    }

    private static string GetFolderPath(Environment.SpecialFolder folder)
    {
        return Environment.GetFolderPath(folder);
    }
}