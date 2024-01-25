using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using Gml.Client;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.ViewModels.Base;
using Gml.Launcher.ViewModels.Components;
using Gml.WebApi.Models.Dtos.Profiles;
using Gml.WebApi.Models.Enums.System;
using ReactiveUI;
using Splat;

namespace Gml.Launcher.ViewModels.Pages;

public class OverviewPageViewModel : PageViewModelBase
{
    private readonly IScreen _screen;
    private readonly IGmlClientManager _clientManager;
    public new string Title => LocalizationService.GetString(ResourceKeysDictionary.MainPageTitle);

    public ReactiveCommand<Unit, IRoutableViewModel> GoProfileCommand { get; set; }
    public ReactiveCommand<Unit, IRoutableViewModel> LogoutCommand { get; set; }
    public ICommand PlayCommand { get; set; }
    public ListViewModel ListViewModel { get; } = new();

    internal OverviewPageViewModel(IScreen screen, IGmlClientManager? clientManager = null) : base(screen)
    {
        _screen = screen;
        _clientManager = clientManager
                         ?? Locator.Current.GetService<IGmlClientManager>()
                         ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));

        GoProfileCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new ProfilePageViewModel(screen))
        );

        LogoutCommand = ReactiveCommand.CreateFromObservable(
            () => screen.Router.Navigate.Execute(new LoginPageViewModel(screen))
        );

        PlayCommand = ReactiveCommand.CreateFromTask(StartGame);

        RxApp.MainThreadScheduler.Schedule(LoadData);
    }

    private async Task StartGame(CancellationToken arg)
    {
        var localProfile = new ProfileCreateInfoDto
        {
            ClientName = ListViewModel.SelectedProfile!.Name,
            GameAddress = "207.180.231.31",
            GamePort = 25565,
            RamSize = 8192,
            IsFullScreen = false,
            OsType = (int)OsType.Windows,
            OsArchitecture = Environment.Is64BitOperatingSystem ? "64" : "32",
            UserAccessToken = "sergsecgrfsecgriseuhcygrshecngrysicugrbn7csewgrfcsercgser",
            UserName = "GamerVII",
            UserUuid = "sergsecgrfsecgriseuhcygrshecngrysicugrbn7csewgrfcsercgser"
        };

        var profileInfo = await _clientManager.GetProfileInfo(localProfile);

        if (profileInfo != null)
        {
            await _clientManager.DownloadNotInstalledFiles(profileInfo);

            var process = await _clientManager.GetProcess(profileInfo);

            process.Start();
            // var p = new ProcessUtil(process);
            // p.OutputReceived += (s, e) => Console.WriteLine(e);
            // p.StartWithEvents();
            // await p.WaitForExitTaskAsync();
            // process.Start();
        }
    }


    private async void LoadData()
    {
        try
        {
            ListViewModel.Profiles = new ObservableCollection<ReadProfileDto>(await _clientManager.GetProfiles());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // ToDo: Send To service
        }
    }
}
