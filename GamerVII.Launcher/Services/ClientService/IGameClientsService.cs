using GamerVII.Launcher.Models.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamerVII.Launcher.Services.ClientService;

/// <summary>
/// Represents a service for managing game clients.
/// </summary>
public interface IGameClientService
{
    /// <summary>
    /// Retrieves a collection of game clients asynchronously.
    /// </summary>
    /// <returns>An asynchronous operation that yields a collection of IGameClient instances.</returns>
    Task<IEnumerable<IGameClient>> GetClientsAsync();
}
