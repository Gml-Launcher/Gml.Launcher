using System;
using System.Collections.ObjectModel;
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
using GmlCore.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Gml.Launcher.ViewModels.Pages;

public class SettingsPageViewModel : PageViewModelBase
{
    private readonly IStorageService _storageService;

    [Reactive]
    public bool DynamicRamValue { get; set; }

    [Reactive]
    public bool FullScreen { get; set; }

    [Reactive]
    public ulong MaxRamValue { get; set; }

    [Reactive]
    public Language? SelectedLanguage { get; set; }

    [Reactive]
    public string? InstallationFolder { get; set; }

    public ICommand ChangeInstallationDirectory { get; }


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
        set
        {
            var isNumeric = int.TryParse(string.Concat(value.Where(char.IsDigit)), out int numericValue);
            if (isNumeric)
            {
                this.RaiseAndSetIfChanged(ref _windowWidth, numericValue);
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _windowWidth, default);
            }
        }
    }

    public string WindowHeight
    {
        get => _windowHeight.ToString();
        set
        {
            var isNumeric = int.TryParse(string.Concat(value.Where(char.IsDigit)), out int numericValue);
            if (isNumeric)
            {
                this.RaiseAndSetIfChanged(ref _windowHeight, numericValue);
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _windowWidth, default);
            }
        }
    }

    public ObservableCollection<Language> AvailableLanguages { get; }

    private int _windowWidth;
    private int _windowHeight;
    private double _ramValue;
    private readonly IGmlClientManager _gmlManager;

    public SettingsPageViewModel(
        IScreen screen,
        ILocalizationService? localizationService,
        IStorageService storageService,
        ISystemService systemService,
        IGmlClientManager gmlManager,
        ProfileReadDto selectedProfile) : base(screen, localizationService)
    {
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

                Thread.CurrentThread.CurrentCulture = language.Culture;
                Thread.CurrentThread.CurrentCulture.ClearCachedData();

                GoBackCommand.Execute(Unit.Default);
            });

        ChangeInstallationDirectory = ReactiveCommand.Create(ChangeFolder);

        AvailableLanguages = new ObservableCollection<Language>(systemService.GetAvailableLanguages());

        MaxRamValue = systemService.GetMaxRam();

        InstallationFolder = _gmlManager.InstallationDirectory;

        RxApp.MainThreadScheduler.Schedule(LoadSettings);
    }

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
        (double ramValue, string width, string height, bool isFullScreen, bool isDynamicRam, Language? selectedLanguage) update)
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
