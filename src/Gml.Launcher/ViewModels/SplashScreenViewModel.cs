using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls.Shapes;
using Gml.Client;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using Gml.Web.Api.Domains.System;
using GmlCore.Interfaces;
using GmlCore.Interfaces.Storage;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Path = Avalonia.Controls.Shapes.Path;

namespace Gml.Launcher.ViewModels;

public class SplashScreenViewModel : WindowViewModelBase
{
    private readonly ISystemService _systemService;
    private readonly IGmlClientManager _manager;
    private readonly ILocalizationService _localizationService;

    [Reactive] public string StatusText { get; set; }
    [Reactive] public bool InfinityLoading { get; set; } = true;
    [Reactive] public Int16 Progress { get; set; }

    public SplashScreenViewModel(ISystemService? systemService = null, IGmlClientManager? manager = null, ILocalizationService? localizationService = null)
    {
        _systemService = systemService ?? Locator.Current.GetService<ISystemService>()
            ?? throw new ServiceNotFoundException(typeof(ISystemService));

        _manager = manager ?? Locator.Current.GetService<IGmlClientManager>()
            ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        _localizationService = localizationService ?? Locator.Current.GetService<ILocalizationService>()
            ?? throw new ServiceNotFoundException(typeof(ILocalizationService));

        StatusText = _localizationService.GetString(ResourceKeysDictionary.PreparingLaunch);
    }

    public async Task InitializeAsync()
    {
        var osType = _systemService.GetOsType();

        await _systemService.LoadSystemData();
        ChangeState(_localizationService.GetString(ResourceKeysDictionary.CheckUpdates), true);

        var versionInfo = await CheckActualVersion(osType);

        if (!versionInfo.IsActuallVersion)
        {
            ChangeState(_localizationService.GetString(ResourceKeysDictionary.InstallingUpdates), false);

            string exePath = Process.GetCurrentProcess().MainModule?.FileName
                             ?? throw new Exception(ResourceKeysDictionary.FailedOs);

            var process = _manager.ProgressChanged.Subscribe(
                percentage => Progress = Convert.ToInt16(percentage));

            await _manager.UpdateCurrentLauncher(versionInfo, osType, System.IO.Path.GetFileName(exePath));

            process.Dispose();
        }
    }

    private async Task<(IVersionFile? ActualVersion, bool IsActuallVersion)> CheckActualVersion(OsType osType)
    {
        var actualVersion = await _manager.GetActualVersion(osType);

        if (actualVersion is null)
        {
            return (null, false);
        }

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
