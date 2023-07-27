using System.Threading.Tasks;

namespace GamerVII.Launcher.Services.LocalStorage;

public interface ILocalStorageService
{

    Task SetAsync<T>(string key, T value);

    Task<T> GetAsync<T>(string key);

}
