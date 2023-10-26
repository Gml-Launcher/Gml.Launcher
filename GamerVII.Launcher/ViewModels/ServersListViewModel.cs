using GamerVII.Launcher.Models.Client;
using GamerVII.Launcher.Services.ClientService;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Linq;

namespace GamerVII.Launcher.ViewModels;

/// <summary>
/// View model class for the list of game servers, derived from ViewModelBase.
/// </summary>
public class ServersListViewModel : ViewModelBase
{
    /// <summary>
    /// Delegate for handling the selected server change event.
    /// </summary>
    public delegate void SelectedServerHandler();

    /// <summary>
    /// Event raised when the selected server is changed.
    /// </summary>
    public event SelectedServerHandler SelectedServerChanged;

    /// <summary>
    /// Gets or sets the collection of game clients (servers).
    /// </summary>
    public ObservableCollection<IGameClient> GameClients
    {
        get => _gameClients;
        set => this.RaiseAndSetIfChanged(ref _gameClients, value);
    }

    /// <summary>
    /// Gets or sets the selected game client (server).
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the ServersListViewModel class.
    /// </summary>
    /// <param name="gameClientService">An optional IGameClientService implementation for retrieving game clients.</param>
    public ServersListViewModel(IGameClientService gameClientService = null)
    {
        _gameClientService = gameClientService ?? Locator.Current.GetService<IGameClientService>()!;

        LoadData();
    }

    // Private methods

    /// <summary>
    /// Loads the list of game clients asynchronously.
    /// </summary>
    private async void LoadData()
    {
        var clients = await _gameClientService.GetClientsAsync();

        GameClients = new ObservableCollection<IGameClient>(clients);

        SelectClient = GameClients.FirstOrDefault();
    }
}
