using GamerVII.MinecraftLauncher.Core.SkinViewer;
using GamerVII.MinecraftLauncher.Models.Client;
using GamerVII.MinecraftLauncher.Models.User;
using GamerVII.MinecraftLauncher.Services.Auth;
using GamerVII.MinecraftLauncher.Services.ClientService;
using GamerVII.MinecraftLauncher.Services.Launch;
using GamerVII.MinecraftLauncher.Services.Skin;
using GamerVII.MinecraftLauncher.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GamerVII.MinecraftLauncher.ViewModels;

public class DashboardViewModel : BaseViewModel
{

    private readonly IGameClientService ServerService;
    private readonly ISkinService SkinService;
    private readonly IGameLaunchService GameService;
    private readonly IAuthService AuthService;

    private SkinViewerManager SkinViewerManager;

    #region Пользователь
    private IUser _user;
    public IUser User
    {
        get => _user;
        set => this.RaiseAndSetIfChanged(ref _user, value);
    }
    #endregion
    #region Логин пользователя
    private string _login = string.Empty;
    public string Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }
    #endregion
    #region Список серверов
    private ObservableCollection<IGameClient> _serversList = new ObservableCollection<IGameClient>();
    public ObservableCollection<IGameClient> GameClients
    {
        get => _serversList;
        set => this.RaiseAndSetIfChanged(ref _serversList, value);
    }
    #endregion
    #region Выбранный сервер
    private IGameClient _selectedServer;
    public IGameClient SelectedGameClient
    {
        get => _selectedServer;
        set => this.RaiseAndSetIfChanged(ref _selectedServer, value);
    }
    #endregion
    #region Загружаемый файл
    private string _loadingFile;
    public string LoadingFile
    {
        get => _loadingFile;
        set => this.RaiseAndSetIfChanged(ref _loadingFile, value);
    }
    #endregion
    #region Процент загрузки
    private decimal _loadingPercentage;
    public decimal LoadingPercentage
    {
        get => _loadingPercentage;
        set => this.RaiseAndSetIfChanged(ref _loadingPercentage, value);
    }
    #endregion
    #region Возможность взаимодействия с игровыми клиентами
    public bool CanStartGame
    {
        get => !IsProcessing;
    }
    #endregion
    #region Прозводится запуск игрового клиента
    private bool _isProcessing = false;
    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            this.RaiseAndSetIfChanged(ref _isProcessing, value);
            this.RaisePropertyChanged(nameof(CanStartGame));
        }
    }

    #endregion
    #region Скин пользователя
    private ImageSource _skin;
    public ImageSource Skin
    {
        get => _skin;
        set => this.RaiseAndSetIfChanged(ref _skin, value);
    }
    #endregion

    public DashboardViewModel()
    {
        // ToDo: replace
        ServerService = App.AppHost!.Services.GetService<IGameClientService>()!;
        AuthService = App.AppHost!.Services.GetService<IAuthService>()!;
        SkinService = App.AppHost!.Services.GetService<ISkinService>()!;
        GameService = App.AppHost!.Services.GetService<IGameLaunchService>()!;

        LoadData();
    }

    private async void LoadData()
    {
        var clients = await ServerService.GetClientsAsync();
        GameClients = new ObservableCollection<IGameClient>(clients);

        if (GameClients.Count > 0)
            SelectedGameClient = GameClients.First();

        SkinViewerManager = await SkinService.GetSkinViewerManager("https://ru-minecraft.ru/uploads/posts/2018-01/1516387236_skin_stasicmirza.png");

        Skin = SkinViewerManager.GetFront(32);
    }


    public ReactiveCommand<PasswordBox, Unit> LoginCommand => ReactiveCommand.CreateFromTask<PasswordBox>(async (passwordBox) =>
    {
        IsProcessing = true;

        try
        {
            User = await AuthService.OnLogin(Login, passwordBox.Password);

            if (!User.IsLogin)
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }
        catch (HttpRequestException httpException)
        {
            MessageBox.Show("Сервер не ответил на запрос авторизации");
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {

            IsProcessing = false;
        }


    });

    public ReactiveCommand<Unit, Unit> StartGameCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        IsProcessing = true;

        IGameClient client = await GameService.LoadClient(SelectedGameClient);

        GameService.FileChanged += (fileName) => LoadingFile = fileName;
        GameService.ProgressChanged += (percentage) => LoadingPercentage = percentage;

        GameService.LoadClientEnded += async (isSuccess, message) =>
        {
            if (isSuccess)
                await GameService.LaunchClient(client);
            else
                MessageBox.Show(message);

            IsProcessing = false;

        };

    });


}
