using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using Gml.Client;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
using Gml.Web.Api.Dto.Messages;
using Gml.Web.Api.Dto.Mods;
using Gml.Web.Api.Dto.Profile;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Sentry;
using Splat;

namespace Gml.Launcher.ViewModels.Pages;

public class ModsPageViewModel : PageViewModelBase
{
    private readonly ProfileReadDto _profile;
    private readonly IUser _user;
    private readonly IGmlClientManager _gmlManager;
    private readonly ISystemService _systemService;
    private readonly string _modsDirectory;
    private ResponseMessage<ProfileReadInfoDto?>? _profileInfo;
    [Reactive] public ConcurrentDictionary<string, ModsDetailsInfoDto> OptionalModsDetails { get; set; } = [];
    [Reactive] public ObservableCollection<ExternalModReadDto> ProfileOptionalMods { get; set; } = [];
    [Reactive] public bool ModsListIsEmpty { get; set; }
    [Reactive] public ICommand ChangeOptionalModState { get; set; }

    public ModsPageViewModel(IScreen screen,
        ProfileReadDto profile,
        IUser user,
        IGmlClientManager gmlManager,
        ISystemService systemService) : base(screen)
    {
        _profile = profile;
        _modsDirectory = Path.Combine(gmlManager.InstallationDirectory, "clients", profile.Name, "mods");
        _user = user;
        _gmlManager = gmlManager;
        _systemService = systemService;

        ChangeOptionalModState = ReactiveCommand.Create<ExternalModReadDto>(ChangeModState);

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }

    private async void ChangeModState(ExternalModReadDto mod)
    {
        try
        {
            var file = Directory.GetFiles(_modsDirectory, $"{mod.Name}*").FirstOrDefault();

            var fileNeedLoad = file is null || !File.Exists(file) || new FileInfo(file).Length == 0;

            if (fileNeedLoad && _profileInfo?.Data is { } profileInfo)
            {
                var fileToDownload = profileInfo.Files.FirstOrDefault(c => c.Name == mod.FileName);
                if (fileToDownload != null)
                    await _gmlManager.DownloadFiles([fileToDownload]);
                file = Directory.GetFiles(_modsDirectory, $"{mod.Name}*").FirstOrDefault();
            }

            var isFullLoaded = file is not null && File.Exists(file) && new FileInfo(file).Length > 0;

            if (isFullLoaded && _gmlManager.ToggleOptionalMod(file, mod.IsSelected))
            {
                // ShowSuccess(SystemConstants.Success,
                //     mod.IsSelected
                //         ? LocalizationService.GetString(SystemConstants.ModEnabled)
                //         : LocalizationService.GetString(SystemConstants.ModDisabled));
            }
        }
        catch (Exception exception)
        {
            ShowError(ResourceKeysDictionary.Error, exception.Message);
            SentrySdk.CaptureException(exception);
        }

    }

    private void LoadData()
    {
        _ = ExecuteFromNewThread(async () =>
        {
            try
            {
                var optionalModsInfo = await _gmlManager.GetOptionalModsInfo(_user.AccessToken);
                var optionalMods = await _gmlManager.GetOptionalMods(_profile.Name, _user.AccessToken);
                var enabledMods = GetEnabledMods(_modsDirectory);
                var models = (optionalMods.Data ?? []).Select(c => new ExternalModReadDto(c, enabledMods.Contains(c.Name)));

                Dispatcher.UIThread.Invoke(() =>
                {
                    OptionalModsDetails = new ConcurrentDictionary<string, ModsDetailsInfoDto>(
                        optionalModsInfo.Data?.ToDictionary(mod => mod.Key, mod => mod) ?? []
                    );
                    ProfileOptionalMods = new ObservableCollection<ExternalModReadDto>(models);
                    ModsListIsEmpty = ProfileOptionalMods.Count == 0;
                });

                _profileInfo = await _gmlManager.GetProfileInfo(new ProfileCreateInfoDto
                {
                    ProfileName = _profile.Name,
                    RamSize = 0,
                    OsType = ((int)_systemService.GetOsType()).ToString(),
                    OsArchitecture = _systemService.GetOsArchitecture(),
                    UserAccessToken = _user.AccessToken,
                    UserName = _user.Name,
                    UserUuid = _user.Uuid
                });
            }
            catch (Exception exception)
            {
                SentrySdk.CaptureException(exception);
            }
        });

    }

    private ICollection<string> GetEnabledMods(string modsDirectory)
    {
        if (!Directory.Exists(modsDirectory))
        {
            Directory.CreateDirectory(modsDirectory);
        }

        var optionalMods = Directory.GetFiles(modsDirectory, $"*-optional-mod.jar");

        return optionalMods.Select(Path.GetFileNameWithoutExtension).ToArray()!;
    }
}

public class ExternalModReadDto : ModReadDto
{
    public bool IsSelected { get; set; }

    public string FileName { get; set; }
    public ExternalModReadDto(ModReadDto baseDto, bool isEnabled = false)
    {
        Type = baseDto.Type;
        Name = baseDto.Name;
        FileName = $"{baseDto.Name}.jar";
        Description = baseDto.Description;
        IsSelected = isEnabled;
    }
}
