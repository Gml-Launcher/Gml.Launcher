using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Gml.Client.Helpers;
using Gml.Client.Models;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Services;
using Gml.Launcher.Models;
using Gml.Launcher.ViewModels.Base;
using Gml.Web.Api.Dto.Profile;
using Microsoft.Win32;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Gml.Launcher.ViewModels.Pages;

public class ModListPageViewModel : PageViewModelBase
{
    private readonly ISystemService _systemService;
    private readonly ProfileReadDto _profile;
    public ICommand OpenFolder { get; set; }
    public ICommand ChooseMod { get; set; }

    public ModListPageViewModel(
        IScreen screen,
        ILocalizationService? localizationService,
        IStorageService storageService,
        ISystemService systemService,
        ProfileReadDto selectedProfile) : base(screen, localizationService)
    {
        _systemService = systemService;
        _profile = selectedProfile;

        OpenFolder = ReactiveCommand.CreateFromTask(Folder);
    }

    private async Task Folder(CancellationToken arg)
    {
        var profilePath = Path.Combine(_systemService.GetApplicationFolder());

        var directoryInfo = new DirectoryInfo(profilePath);

        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        // combine the arguments together
        // it doesn't matter if there is a space after ','
        string argument = "/select, \"" + profilePath + "\"";

        Process.Start("explorer.exe", argument);
    }
}
