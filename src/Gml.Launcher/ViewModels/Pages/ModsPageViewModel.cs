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
using Gml.Launcher.Core;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Gml.Launcher.ViewModels.Base;
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
    private readonly string _modsDirectory;
    [Reactive] public ConcurrentDictionary<string, ModsDetailsInfoDto> OptionalModsDetails { get; set; } = [];
    [Reactive] public ObservableCollection<ExternalModReadDto> ProfileOptionalMods { get; set; } = [];
    [Reactive] public bool ModsListIsEmpty { get; set; }
    [Reactive] public ICommand ChangeOptionalModState { get; set; }

    public ModsPageViewModel(IScreen screen,
        ProfileReadDto profile,
        IUser user,
        IGmlClientManager gmlManager) : base(screen)
    {
        _profile = profile;
        _modsDirectory = Path.Combine(gmlManager.InstallationDirectory, "clients", profile.Name, "mods");
        _user = user;
        _gmlManager = gmlManager;

        ChangeOptionalModState = ReactiveCommand.Create<ExternalModReadDto>(ChangeModState);

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }

    private void ChangeModState(ExternalModReadDto mod)
    {
        var files = Directory.GetFiles(_modsDirectory, $"{mod.Name}*").FirstOrDefault();

        if (!string.IsNullOrEmpty(files) && _gmlManager.ToggleOptionalMod(files, mod.IsSelected))
        {
            // ShowSuccess(SystemConstants.Success,
            //     mod.IsSelected
            //         ? LocalizationService.GetString(SystemConstants.ModEnabled)
            //         : LocalizationService.GetString(SystemConstants.ModDisabled));
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
            }
            catch (Exception exception)
            {
                SentrySdk.CaptureException(exception);
            }
        });

    }

    private ICollection<string> GetEnabledMods(string modsDirectory)
    {
        var optionalMods = Directory.GetFiles(modsDirectory, $"*-optional-mod.jar");

        return optionalMods.Select(Path.GetFileNameWithoutExtension).ToArray()!;
    }
}

public class ExternalModReadDto : ModReadDto
{
    public bool IsSelected { get; set; }
    public ExternalModReadDto(ModReadDto baseDto, bool isEnabled = false)
    {
        Type = baseDto.Type;
        Name = baseDto.Name;
        Description = baseDto.Description;
        IsSelected = isEnabled;
    }

}
