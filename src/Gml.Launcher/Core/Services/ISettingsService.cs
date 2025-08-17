using System.Collections.Generic;
using System.Threading.Tasks;
using Gml.Launcher.Models;

namespace Gml.Launcher.Core.Services;

public interface ISettingsService
{
    public Task<SettingsInfo> GetSettings();
    Task UpdateSettingsAsync(SettingsInfo newSettings);
    IEnumerable<Language> GetAvailableLanguages();
    ulong GetMaxRam();
    Task UpdateInstallationDirectory(string? installationFolder);
}
