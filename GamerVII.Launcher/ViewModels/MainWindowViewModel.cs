using DynamicData;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Models.Users;
using GamerVII.Launcher.Services.AuthService;
using GamerVII.Launcher.Services.GameLaunchService;
using GamerVII.Launcher.Services.LoggerService;
using Splat;

namespace GamerVII.Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public SidebarViewModel SidebarViewModel { get; }

        #region Public properties

        #region Current user

        public IUser User
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }

        #endregion

        #region Processing

        public bool IsProcessing
        {
            get => _isProcessing;
            set => this.RaiseAndSetIfChanged(ref _isProcessing, value);
        }

        #endregion

        #region Текущая страница

        public PageViewModelBase CurrentPage
        {
            get => _currentPage;
            private set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        #endregion

        #region Processing file name

        public string LoadingFile
        {
            get => _loadingFile;
            set => this.RaiseAndSetIfChanged(ref _loadingFile, value);
        }

        #endregion

        #region Processing percentage

        public decimal LoadingPercentage
        {
            get => _loadingPercentage;
            set => this.RaiseAndSetIfChanged(ref _loadingPercentage, value);
        }

        #endregion

        #endregion

        #region Commands

        public ICommand LaunchGameCommand { get; }
        public ICommand SettingsClientCommand { get; }
        public ICommand ModsListCommand { get; }

        #endregion

        #region Private variables

        private readonly IAuthService _authService;
        private readonly IGameLaunchService _gameLaunchService;
        private readonly ILoggerService _loggerService;

        private bool _isProcessing = false;
        private string _loadingFile = string.Empty;
        private decimal _loadingPercentage = 0;
        private IUser _user;
        private PageViewModelBase _currentPage;

        private readonly PageViewModelBase[] Pages =
        {
            new AuthPageViewModel(),
            new ProfilePageViewModel(),
            new ClientSettingsPageViewModel(),
        };

        #endregion


        public MainWindowViewModel(IGameLaunchService gameLaunchService = null, IAuthService? authService = null, ILoggerService loggerService = null)
        {
            SidebarViewModel = new SidebarViewModel();

            _gameLaunchService = gameLaunchService ?? Locator.Current.GetService<IGameLaunchService>()!;
            _authService = authService ?? Locator.Current.GetService<IAuthService>()!;
            _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>()!;

            var canLaunch = this.WhenAnyValue(
                x => x.IsProcessing, x => x.SidebarViewModel.ServersListViewModel.SelectClient,
                (isProcessing, gameClient) =>
                    isProcessing == false &&
                    gameClient != null
            );

            var canViewMods = this.WhenAnyValue(
                x => x.IsProcessing, x => x.SidebarViewModel.ServersListViewModel.SelectClient,
                (isProcessing, gameClient) => gameClient != null
            );

            SidebarViewModel.OpenProfilePageCommand = ReactiveCommand.Create(OpenPage<ProfilePageViewModel>);
            SidebarViewModel.LogoutCommand = ReactiveCommand.CreateFromTask(Logout);
            SettingsClientCommand = ReactiveCommand.Create(OpenPage<ClientSettingsPageViewModel>, canLaunch);
            ModsListCommand = ReactiveCommand.Create(() => OpenPage<ClientSettingsPageViewModel>(), canViewMods);
            LaunchGameCommand = ReactiveCommand.CreateFromTask(LaunchGame, canLaunch);
            SidebarViewModel.ServersListViewModel.SelectedServerChanged += ResetPage;

            LoadData();
        }

        private async Task Logout()
        {
            await _authService.OnLogout();
            User = null;

            OpenPage<AuthPageViewModel>();
        }

        private async void LoadData()
        {
            User = await _authService.GetAuthorizedUser();

            if (User == null || !User.IsLogin)
                OpenPage<AuthPageViewModel>();

            var authPage = Pages.FirstOrDefault(c => c.GetType().Equals(typeof(AuthPageViewModel))) as AuthPageViewModel;
            var profilePage =  Pages.FirstOrDefault(c => c.GetType().Equals(typeof(ProfilePageViewModel))) as ProfilePageViewModel;
            var settingsPage =  Pages.FirstOrDefault(c => c.GetType().Equals(typeof(ClientSettingsPageViewModel))) as ClientSettingsPageViewModel;

            profilePage.GoTaMainPageCommand = ReactiveCommand.Create(ResetPage);
            settingsPage.GoTaMainPageCommand = ReactiveCommand.Create(ResetPage);

            authPage.Authorized += (isSuccess) =>
            {
                if (isSuccess)
                {
                    ResetPage();
                }
            };
        }

        private void ResetPage()
        {
            CurrentPage = null;
        }

        private void OpenPage<T>()
        {
            var type = typeof(T);

            var page = Pages.FirstOrDefault(c => c.GetType().Equals(type));

            var index = Pages.IndexOf(page);

            if (index != -1)
            {
                CurrentPage = Pages[index];
            }
        }

        private async Task LaunchGame(CancellationToken arg)
        {
            try
            {
                IsProcessing = true;

                IGameClient client =
                    await _gameLaunchService.LoadClient(SidebarViewModel.ServersListViewModel.SelectClient);

                _gameLaunchService.FileChanged += (fileName) => LoadingFile = fileName;
                _gameLaunchService.ProgressChanged += (percentage) => LoadingPercentage = percentage;

                _gameLaunchService.LoadClientEnded += async (isSuccess, message) =>
                {
                    if (isSuccess)
                    {
                        Process process = await _gameLaunchService.LaunchClient(client, User);

                        process.Exited += async (sender, e) =>
                        {
                            await Dispatcher.UIThread.InvokeAsync(() => IsProcessing = false);
                        };
                    }
                    else
                    {
                        await Dispatcher.UIThread.InvokeAsync(() => IsProcessing = false);
                        Console.WriteLine(message);
                    }

                };
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex.Message);
                _loggerService.Log(ex.StackTrace);
            }
        }
    }
}