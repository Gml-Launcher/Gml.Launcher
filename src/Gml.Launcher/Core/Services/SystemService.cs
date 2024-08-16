using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Gml.Launcher.Models;
using Gml.Web.Api.Domains.System;
using Hardware.Info;
using static System.OperatingSystem;

namespace Gml.Launcher.Core.Services;

public class SystemService : ISystemService
{
    private const string NotSupportedMessage = "The operating system is not supported.";
    private readonly HardwareInfo _hardwareInfo = new();
    const int ERROR_DISK_FULL = 0x70;

    public ulong GetMaxRam()
    {
        if (!(IsWindows() || IsLinux() || IsMacOS())) throw new NotSupportedException(NotSupportedMessage);

        _hardwareInfo.RefreshMemoryStatus();

        return _hardwareInfo.MemoryStatus.TotalPhysical / (1024 * 1024);
    }

    public string GetApplicationFolder()
    {
        if (IsWindows()) return Path.GetFullPath(GetFolderPath(Environment.SpecialFolder.ApplicationData));

        if (IsLinux() || IsMacOS()) return Path.GetFullPath(GetFolderPath(Environment.SpecialFolder.UserProfile));

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
        var refreshDriveListTask = Task.Run(() => _hardwareInfo.RefreshDriveList());
        var refreshMotherboardListTask = Task.Run(() => _hardwareInfo.RefreshMotherboardList());
        var refreshCpuListTask = Task.Run(() => _hardwareInfo.RefreshCPUList());

        await Task.WhenAll(refreshDriveListTask, refreshMotherboardListTask, refreshCpuListTask);
    }

    public bool IsDiskFull(IOException ioException)
    {
        int hr = Marshal.GetHRForException(ioException);
        return (hr & 0xFFFF) == ERROR_DISK_FULL;
    }

    public OsType GetOsType()
    {
        if (IsWindows()) return OsType.Windows;

        if (IsLinux()) return OsType.Linux;

        if (IsMacOS()) return OsType.OsX;

        return OsType.Undefined;
    }

    public IEnumerable<Language> GetAvailableLanguages()
    {
        return
        [
            new() { IconPath = "/Assets/Images/lang-ru.svg", Name = "Русский", Culture = new CultureInfo("ru-RU") },
            new() { IconPath = "/Assets/Images/lang-us.svg", Name = "English", Culture = new CultureInfo("en-US") }
        ];
    }

    private static string GetFolderPath(Environment.SpecialFolder folder)
    {
        return Environment.GetFolderPath(folder);
    }
}
