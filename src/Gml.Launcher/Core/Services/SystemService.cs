using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gml.Launcher.Models;
using Gml.Web.Api.Domains.System;
using Hardware.Info;
using static System.OperatingSystem;

namespace Gml.Launcher.Core.Services;

public class SystemService : ISystemService
{
    private readonly HardwareInfo _hardwareInfo;
    private const string NotSupportedMessage = "The operating system is not supported.";

    public SystemService()
    {
        _hardwareInfo = new HardwareInfo();
    }

    public ulong GetMaxRam()
    {
        if (!(IsWindows() || IsLinux() || IsMacOS()))
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        _hardwareInfo.RefreshMemoryStatus();

        return _hardwareInfo.MemoryStatus.TotalPhysical / (1024 * 1024);
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
        var directoryInfo = new DirectoryInfo(Path.Combine(GetApplicationFolder(), additionalPath));

        if (needCreate && !directoryInfo.Exists)
            directoryInfo.Create();

        return directoryInfo.FullName;
    }

    public string GetHwid()
    {
        var cpuIdentifier = _hardwareInfo.CpuList.FirstOrDefault()?.ProcessorId ?? "NOT_FOUND";
        var motherboardIdentifier = _hardwareInfo.MotherboardList.FirstOrDefault()?.SerialNumber ?? "NOT_FOUND";

        var diskIdentifiers = string.Join("-", _hardwareInfo.DriveList.Select(d => d.SerialNumber));

        return $"{cpuIdentifier}-{motherboardIdentifier}-{diskIdentifiers}";
    }

    public async Task LoadSystemData()
    {
        await Task.Run(() =>
        {
            _hardwareInfo.RefreshDriveList();
            _hardwareInfo.RefreshMotherboardList();
            _hardwareInfo.RefreshCPUList();
        });
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
