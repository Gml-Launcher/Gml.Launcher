using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Gml.Launcher.Models;
using Gml.Web.Api.Domains.System;
using Hardware.Info;
using static System.OperatingSystem;

namespace Gml.Launcher.Core.Services;

public class SystemService : ISystemService
{
    private const string NotSupportedMessage = "The operating system is not supported.";

    public SystemService()
    {
    }

    public ulong GetMaxRam()
    {
        if (!(IsWindows() || IsLinux() || IsMacOS()))
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        var hardwareInfo = new HardwareInfo();

        hardwareInfo.RefreshMemoryStatus();

        return hardwareInfo.MemoryStatus.TotalPhysical / (1024 * 1024);
    }

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

    public string GetGameFolder(string additionalPath, bool needCreate)
    {
        var directoryInfo = new DirectoryInfo(Path.Combine(GetApplicationFolder(), additionalPath)); // ToDo: To const

        if (needCreate && !directoryInfo.Exists)
            directoryInfo.Create();

        return directoryInfo.FullName;
    }

    private static string GetFolderPath(Environment.SpecialFolder folder)
    {
        return Environment.GetFolderPath(folder);
    }

    public OsType GetOsType()
    {
        if (IsWindows())
        {
            return OsType.Windows;
        }

        if (IsLinux())
        {
            return OsType.Linux;
        }

        if (IsMacOS())
        {
            return OsType.OsX;
        }

        return OsType.Undefined;
    }

    public IEnumerable<Language> GetAvailableLanguages()
    {
       return new List<Language>
        {
            new() { IconPath = "/Assets/Images/lang-ru.svg", Name = "Русский", Culture = new CultureInfo("ru-RU") },
            new() { IconPath = "/Assets/Images/lang-us.svg", Name = "English", Culture = new CultureInfo("en-US") },
        };
    }
}
