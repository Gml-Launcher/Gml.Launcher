using System.Threading.Tasks;

namespace Gml.Launcher.Core.Services;

public interface IBackendChecker
{
    Task<bool> BackendIsActive();
    Task UpdateBackendStatus();
    bool IsOffline { get; }
}
