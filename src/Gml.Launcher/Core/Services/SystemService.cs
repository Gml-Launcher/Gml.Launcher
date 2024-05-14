using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using Avalonia.Controls.Shapes;
using Gml.Client;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Models;
using Gml.Web.Api.Domains.System;
using Splat;
using static System.OperatingSystem;

namespace Gml.Launcher.Core.Services;

public class SystemService : ISystemService
{
    private const string NotSupportedMessage = "Operating system not supported";

    public SystemService()
    {
    }


    public ulong GetMaxRam()
    {
        if (IsWindows())
        {
            return GetWindowsMaxRam();
        }

        if (IsLinux())
        {
            return GetUnixMaxRam();
        }

        throw new NotSupportedException("The operating system is not supported.");
    }
    private static ulong GetWindowsMaxRam()
    {
        var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
        var collection = searcher.Get();

        foreach (var item in collection)
        {
            ulong totalPhysicalMemory = (ulong)item["TotalPhysicalMemory"];
            return totalPhysicalMemory / (1024 * 1024);
        }

        throw new Exception("Unable to determine total physical memory.");
    }

    private static ulong GetUnixMaxRam()
    {
        try
        {
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \"free -b | grep Mem | awk '{print $2}'\"";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.Start();

                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                if(ulong.TryParse(output, out ulong totalPhysicalMemory))
                {
                    return totalPhysicalMemory / (1024 * 1024);
                }
                else
                {
                    throw new Exception("Unable to parse total physical memory.");
                }
            }
        }
        catch (Exception ex)
        {
            // Handle or log exception as needed
            throw new Exception("Unable to determine total physical memory.", ex);
        }
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

    public string GetGameFolder(string addtionalPath, bool needCreate)
    {
        var directoryInfo =
            new DirectoryInfo(System.IO.Path.Combine(GetApplicationFolder(), addtionalPath)); // ToDo: To const

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
