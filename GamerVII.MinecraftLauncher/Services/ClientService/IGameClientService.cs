using GamerVII.MinecraftLauncher.Models.Client;
using GamerVII.MinecraftLauncher.Services.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamerVII.MinecraftLauncher.Services.ClientService;

public interface IGameClientService : IServiceBase
{
    Task<IEnumerable<IGameClient>> GetClientsAsync();
}
