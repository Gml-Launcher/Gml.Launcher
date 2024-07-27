using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Gml.Client;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using Gml.Web.Api.Domains.System;
using GmlCore.Interfaces.Storage;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Gml.Launcher.ViewModels;

public class SplashScreenViewModel : WindowViewModelBase
{
    private readonly ILocalizationService _localizationService;
    private readonly IGmlClientManager _manager;
    private readonly ISystemService _systemService;

    public SplashScreenViewModel(ISystemService? systemService = null, IGmlClientManager? manager = null,
        ILocalizationService? localizationService = null)
    {
        _systemService = systemService ?? Locator.Current.GetService<ISystemService>()
            ?? throw new ServiceNotFoundException(typeof(ISystemService));

        _manager = manager ?? Locator.Current.GetService<IGmlClientManager>()
            ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        _localizationService = localizationService ?? Locator.Current.GetService<ILocalizationService>()
            ?? throw new ServiceNotFoundException(typeof(ILocalizationService));

        StatusText = _localizationService.GetString(ResourceKeysDictionary.PreparingLaunch);
    }

    [Reactive] public string StatusText { get; set; }
    [Reactive] public bool InfinityLoading { get; set; } = true;
    [Reactive] public short Progress { get; set; }

    public async Task InitializeAsync()
    {
        var osType = _systemService.GetOsType();
        var osArch = RuntimeInformation.ProcessArchitecture;

        await _systemService.LoadSystemData();
        ChangeState(_localizationService.GetString(ResourceKeysDictionary.CheckUpdates), true);

        var versionInfo = await CheckActualVersion(osType, osArch);

        if (!versionInfo.IsActuallVersion)
        {
            ChangeState(_localizationService.GetString(ResourceKeysDictionary.InstallingUpdates), false);

            var exePath = Process.GetCurrentProcess().MainModule?.FileName
                          ?? throw new Exception(ResourceKeysDictionary.FailedOs);

            var process = _manager.ProgressChanged.Subscribe(
                percentage => Progress = Convert.ToInt16(percentage));

            await _manager.UpdateCurrentLauncher(versionInfo, osType, Path.GetFileName(exePath));

            process.Dispose();
        }
    }

    private async Task<(IVersionFile? ActualVersion, bool IsActuallVersion)> CheckActualVersion(OsType osType,
        Architecture osArch)
    {
        var actualVersion = await _manager.GetActualVersion(osType, osArch);

        if (actualVersion is null) return (null, true);

        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0, 0);

        return actualVersion.Version.Equals(version.ToString())
            ? (actualVersion, true)
            : (actualVersion, false);
    }

    private void ChangeState(string text, bool isInfinity)
    {
        StatusText = text;
        InfinityLoading = isInfinity;
    }
}
