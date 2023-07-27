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
    /// <summary>
    /// View model class for the main window, derived from ViewModelBase.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        // Sidebar view model for the main window.
        public SidebarViewModel SidebarViewModel { get; }

        #region Public properties

        #region Current user

        /// <summary>
        /// Gets or sets the currently authorized user.
        /// </summary>
        public IUser User
        {
            get => _user;
            set => this.RaiseAndSetIfChanged(ref _user, value);
        }

        #endregion

        #region Processing

        /// <summary>
        /// Gets or sets a value indicating whether an operation is in progress.
        /// </summary>
        public bool IsProcessing
        {
            get => _isProcessing;
            set => this.RaiseAndSetIfChanged(ref _isProcessing, value);
        }

        #endregion

        #region Current page

        /// <summary>
        /// Gets or sets the current page view model displayed in the main window.
        /// </summary>
        public PageViewModelBase CurrentPage
        {
            get => _currentPage;
            private set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        #endregion

        #region Processing file name

        /// <summary>
        /// Gets or sets the name of the file currently being processed.
        /// </summary>
        public string LoadingFile
        {
            get => _loadingFile;
            set => this.RaiseAndSetIfChanged(ref _loadingFile, value);
        }

        #endregion

        #region Processing percentage

        /// <summary>
        /// Gets or sets the percentage of the processing progress.
        /// </summary>
        public decimal LoadingPercentage
        {
            get => _loadingPercentage;
            set => this.RaiseAndSetIfChanged(ref _loadingPercentage, value);
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Command to launch the game.
        /// </summary>
        public ICommand LaunchGameCommand { get; }

        /// <summary>
        /// Command to open the client settings page.
        /// </summary>
        public ICommand SettingsClientCommand { get; }

        /// <summary>
        /// Command to view the list of mods.
        /// </summary>
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

        // Array of available page view models.
        private readonly PageViewModelBase[] Pages =
        {
            new AuthPageViewModel(),
            new ProfilePageViewModel(),
            new ClientSettingsPageViewModel(),
        };

        #endregion


        /// <summary>
        /// Initializes a new instance of the MainWindowViewModel class.
        /// </summary>
        /// <param name="gameLaunchService">An optional IGameLaunchService implementation for game launching.</param>
        /// <param name="authService">An optional IAuthService implementation for user authentication.</param>
        /// <param name="loggerService">An optional ILoggerService implementation for logging messages.</param>
        public MainWindowViewModel(IGameLaunchService gameLaunchService = null, IAuthService? authService = null, ILoggerService loggerService = null)
        {
            // Initialize the SidebarViewModel for the main window.
            SidebarViewModel = new SidebarViewModel();

            // Set the provided or default implementations for game launch service, authentication service, and logger service.
            _gameLaunchService = gameLaunchService ?? Locator.Current.GetService<IGameLaunchService>()!;
            _authService = authService ?? Locator.Current.GetService<IAuthService>()!;
            _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>()!;

            // Define conditions for enabling certain commands based on view model properties.
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

            // Set up commands with corresponding actions and conditions.
            SidebarViewModel.OpenProfilePageCommand = ReactiveCommand.Create(OpenPage<ProfilePageViewModel>);
            SidebarViewModel.LogoutCommand = ReactiveCommand.CreateFromTask(Logout);
            SettingsClientCommand = ReactiveCommand.Create(OpenPage<ClientSettingsPageViewModel>, canLaunch);
            ModsListCommand = ReactiveCommand.Create(() => OpenPage<ClientSettingsPageViewModel>(), canViewMods);
            LaunchGameCommand = ReactiveCommand.CreateFromTask(LaunchGame, canLaunch);
            SidebarViewModel.ServersListViewModel.SelectedServerChanged += ResetPage;

            // Load user data and set the appropriate initial page based on user status.
            LoadData();
        }

        /// <summary>
        /// Handles the logout operation.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        private async Task Logout()
        {
            await _authService.OnLogout();
            User = null;

            OpenPage<AuthPageViewModel>();
        }

        /// <summary>
        /// Loads initial data, sets the user, and determines the initial page to display.
        /// </summary>
        private async void LoadData()
        {
            User = await _authService.GetAuthorizedUser();

            if (User == null || !User.IsLogin)
                OpenPage<AuthPageViewModel>();

            // Obtain instances of the AuthPageViewModel, ProfilePageViewModel, and ClientSettingsPageViewModel
            // to set up navigation commands between pages.
            var authPage = Pages.FirstOrDefault(c => c.GetType().Equals(typeof(AuthPageViewModel))) as AuthPageViewModel;
            var profilePage =  Pages.FirstOrDefault(c => c.GetType().Equals(typeof(ProfilePageViewModel))) as ProfilePageViewModel;
            var settingsPage =  Pages.FirstOrDefault(c => c.GetType().Equals(typeof(ClientSettingsPageViewModel))) as ClientSettingsPageViewModel;

            // Set the GoToMainPageCommand for profile and client settings pages to reset the current page.
            profilePage.GoToMainPageCommand = ReactiveCommand.Create(ResetPage);
            settingsPage.GoToMainPageCommand = ReactiveCommand.Create(ResetPage);

            // Subscribe to the Authorized event of the AuthPageViewModel to handle successful authorization.
            authPage.Authorized += (isSuccess) =>
            {
                if (isSuccess)
                {
                    ResetPage();
                }
            };
        }

        /// <summary>
        /// Resets the current page by setting it to null.
        /// </summary>
        private void ResetPage()
        {
            CurrentPage = null;
        }

        /// <summary>
        /// Opens the specified page view model and sets it as the current page.
        /// </summary>
        /// <typeparam name="T">The type of the page view model to open.</typeparam>
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

        /// <summary>
        /// Launches the game based on the selected game client.
        /// </summary>
        /// <param name="arg">The cancellation token for the async task.</param>
        /// <returns>An asynchronous task.</returns>
        private async Task LaunchGame(CancellationToken arg)
        {
            try
            {
                IsProcessing = true;

                IGameClient client =
                    await _gameLaunchService.LoadClient(SidebarViewModel.ServersListViewModel.SelectClient);

                // Subscribe to events from the game launch service to update processing information.
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
