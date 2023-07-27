using GamerVII.Launcher.Models.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamerVII.Launcher.Services.ClientService;

public interface IGameClientService
{
    Task<IEnumerable<IGameClient>> GetClientsAsync();
}