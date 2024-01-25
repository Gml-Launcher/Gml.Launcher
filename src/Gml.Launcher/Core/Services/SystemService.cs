using System;
using System.Runtime.InteropServices;

namespace Gml.Launcher.Core.Services;

public class SystemService : ISystemService
{
    private const string NotSupportedMessage = "Operating system not supported";

    public string GetApplicationFolder()
    {
        var os = GetOperatingSystem();

        if (os == OSPlatform.Windows)
        {
            return GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        if (os == OSPlatform.Linux || os == OSPlatform.OSX)
        {
            return GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        throw new NotSupportedException(NotSupportedMessage);
    }

    private static string GetFolderPath(Environment.SpecialFolder folder)
    {
        return Environment.GetFolderPath(folder);
    }

    private static OSPlatform? GetOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OSPlatform.Windows;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OSPlatform.Linux;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OSPlatform.OSX;
        }
        return null;
    }
}
