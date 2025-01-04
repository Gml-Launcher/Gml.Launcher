using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using Gml.Client;
using Gml.Launcher.Core.Services;
using Gml.Launcher.Models;
using Gml.Launcher.ViewModels.Base;
using Gml.Web.Api.Dto.Profile;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Gml.Launcher.ViewModels.Pages;

public class SettingsPageViewModel : PageViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly IGmlClientManager _gmlManager;
    private readonly IStorageService _storageService;
    private double _ramValue;
    private int _windowHeight;

    private int _windowWidth;

    public SettingsPageViewModel(
        IScreen screen,
        ILocalizationService? localizationService,
        IStorageService storageService,
        ISystemService systemService,
        IGmlClientManager gmlManager,
        ProfileReadDto selectedProfile) : base(screen, localizationService)
    {
        _mainViewModel = (MainWindowViewModel)screen;
        _storageService = storageService;
        _gmlManager = gmlManager;

        this.WhenAnyValue(
                x => x.RamValue,
                x => x.WindowWidth,
                x => x.WindowHeight,
                x => x.FullScreen,
                x => x.DynamicRamValue,
                x => x.SelectedLanguage
            )
            .Throttle(TimeSpan.FromMilliseconds(400))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(SaveSettings);

        this.WhenAnyValue(x => x.SelectedLanguage)
            .Skip(2)
            .Subscribe(language =>
            {
                if (language == null) return;

                Assets.Resources.Resources.Culture = language.Culture;

                GoBackCommand.Execute(Unit.Default);
            });

        ChangeInstallationDirectory = ReactiveCommand.Create(ChangeFolder);

        AvailableLanguages = new ObservableCollection<Language>(systemService.GetAvailableLanguages());

        MaxRamValue = systemService.GetMaxRam();
        const int highRamThreshold = 16384;
        MinRamValue = (ulong)(MaxRamValue > highRamThreshold ? 1024 : 512);
        RamTickValue = (ulong)(MaxRamValue > highRamThreshold ? 1024 : 512);

        InstallationFolder = _gmlManager.InstallationDirectory;

        RxApp.MainThreadScheduler.Schedule(LoadSettings);
    }

    [Reactive] public bool DynamicRamValue { get; set; }

    [Reactive] public bool FullScreen { get; set; }

    [Reactive] public ulong MinRamValue { get; set; }

    [Reactive] public ulong MaxRamValue { get; set; }

    [Reactive] public ulong RamTickValue { get; set; }

    [Reactive] public Language? SelectedLanguage { get; set; }

    [Reactive] public string? InstallationFolder { get; set; }
    [Reactive] public string? RamValueView { get; set; }

    public ICommand ChangeInstallationDirectory { get; }


    public double RamValue
    {
        get => _ramValue;
        set
        {
            if (!(Math.Abs(value - _ramValue) > 0.0)) return;

            _ramValue = RoundToNearest(value, 8);
            RamValueView = Convert.ToInt32(_ramValue).ToString(CultureInfo.InvariantCulture);
            this.RaisePropertyChanged();
        }
    }

    public string WindowWidth
    {
        get => _windowWidth.ToString();
        set
        {
            var isNumeric = int.TryParse(string.Concat(value.Where(char.IsDigit)), out var numericValue);
            if (isNumeric)
                this.RaiseAndSetIfChanged(ref _windowWidth, numericValue);
            else
                this.RaiseAndSetIfChanged(ref _windowWidth, default);
        }
    }

    public string WindowHeight
    {
        get => _windowHeight.ToString();
        set
        {
            var isNumeric = int.TryParse(string.Concat(value.Where(char.IsDigit)), out var numericValue);
            if (isNumeric)
                this.RaiseAndSetIfChanged(ref _windowHeight, numericValue);
            else
                this.RaiseAndSetIfChanged(ref _windowWidth, default);
        }
    }

    public ObservableCollection<Language> AvailableLanguages { get; }
    public MainWindowViewModel MainViewModel => _mainViewModel;

    internal void ChangeFolder()
    {
        if (InstallationFolder != null)
            _gmlManager.ChangeInstallationFolder(InstallationFolder);

        _storageService.SetAsync(StorageConstants.InstallationDirectory, InstallationFolder);
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

        SelectedLanguage = !string.IsNullOrEmpty(data.LanguageCode)
            ? AvailableLanguages.FirstOrDefault(c => c.Culture.Name == data.LanguageCode)
            : AvailableLanguages.FirstOrDefault();

        WindowWidth = data.GameWidth == 0 ? "900" : data.GameWidth.ToString();
        WindowHeight = data.GameHeight == 0 ? "600" : data.GameHeight.ToString();
        RamValue = data.RamValue;
        DynamicRamValue = data.IsDynamicRam;
        FullScreen = data.FullScreen;
    }

    private void SaveSettings(
        (double ramValue, string width, string height, bool isFullScreen, bool isDynamicRam, Language? selectedLanguage)
            update)
    {
        _storageService.SetAsync(
            StorageConstants.Settings,
            new SettingsInfo(
                int.Parse(update.width),
                int.Parse(update.height),
                update.isFullScreen,
                update.isDynamicRam,
                update.ramValue, update.selectedLanguage?.Culture.Name));
    }
}
