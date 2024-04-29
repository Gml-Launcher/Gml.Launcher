using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gml.Launcher.Core.Services;
using Gml.Launcher.Models;
using Gml.Launcher.ViewModels.Base;
using Gml.WebApi.Models.Dtos.Profiles;
using ReactiveUI;

namespace Gml.Launcher.ViewModels.Pages;

public class SettingsPageViewModel : PageViewModelBase
{
    private readonly IStorageService _storageService;
    private readonly ReadProfileDto _selectedProfile;

    public bool DynamicRamValue
    {
        get => _dynamicRamValue;
        set => this.RaiseAndSetIfChanged(ref _dynamicRamValue, value);
    }

    public bool FullScreen
    {
        get => _fullScreen;
        set => this.RaiseAndSetIfChanged(ref _fullScreen, value);
    }


    public double RamValue
    {
        get => _ramValue;
        set
        {
            if (!(Math.Abs(value - _ramValue) > 0.0)) return;

            _ramValue = RoundToNearest(value, 8);
            this.RaisePropertyChanged();
        }
    }

    public string WindowWidth
    {
        get => _windowWidth.ToString();
        set => this.RaiseAndSetIfChanged(ref _windowWidth, int.Parse(string.Concat(value.Where(char.IsDigit))));
    }

    public string WindowHeight
    {
        get => _windowHeight.ToString();
        set => this.RaiseAndSetIfChanged(ref _windowHeight, int.Parse(string.Concat(value.Where(char.IsDigit))));
    }

    private int _windowWidth;
    private int _windowHeight;
    private double _ramValue;
    private bool _dynamicRamValue;
    private bool _fullScreen;
    private bool _isBusy;

    public SettingsPageViewModel(
        IScreen screen,
        ILocalizationService? localizationService,
        IStorageService storageService,
        ReadProfileDto selectedProfile) : base(screen, localizationService)
    {
        _storageService = storageService;
        _selectedProfile = selectedProfile;

        this.WhenAnyValue(
                x => x.RamValue,
                x => x.WindowWidth,
                x => x.WindowHeight,
                x => x.FullScreen,
                x => x.DynamicRamValue
            )
            .Throttle(TimeSpan.FromMilliseconds(400))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(SaveSettings);

        RxApp.MainThreadScheduler.Schedule(LoadSettings);
    }

    private double RoundToNearest(double value, double step)
    {
        var offset = value % step;
        return offset >= step / 2.0
            ? value + (step - offset)
            : value - offset;
    }

    private async void LoadSettings()
    {
        var data = await _storageService.GetAsync<SettingsInfo>(StorageConstants.Settings);

        if (data == null) return;

        WindowWidth = data.GameWidth.ToString();
        WindowHeight = data.GameHeight.ToString();
        RamValue = data.RamValue;
        DynamicRamValue = data.IsDynamicRam;
        FullScreen = data.FullScreen;
    }

    private void SaveSettings(
        (double ramValue, string width, string height, bool isFullScreen, bool isDynamicRam) update)
    {
        _storageService.SetAsync(
            StorageConstants.Settings,
            new SettingsInfo(int.Parse(update.width), int.Parse(update.height), update.isFullScreen,
                update.isDynamicRam, update.ramValue));
    }
}
