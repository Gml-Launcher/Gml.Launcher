using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Services.ClientService;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;

namespace GamerVII.Launcher.ViewModels;

public class ServersListViewModel : ViewModelBase
{
    public delegate void SelectedServerHandler();

    public event SelectedServerHandler SelectedServerChanged;


    public ObservableCollection<IGameClient> GameClients
    {
        get => _gameClients;
        set => this.RaiseAndSetIfChanged(ref _gameClients, value);
    }

    public IGameClient SelectClient
    {
        get => _selectedClient;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedClient, value);
            SelectedServerChanged?.Invoke();
        }
    }

    private readonly IGameClientService _gameClientService;

    private ObservableCollection<IGameClient> _gameClients = null!;
    private IGameClient _selectedClient = null!;

    public ServersListViewModel(IGameClientService gameClientService = null)
    {
        _gameClientService = gameClientService ?? Locator.Current.GetService<IGameClientService>()!;

        LoadData();
    }

    private async void LoadData()
    {
        var clients = await _gameClientService.GetClientsAsync();

        GameClients = new ObservableCollection<IGameClient>(clients);
    }
}