using System.Threading.Tasks;

namespace Gml.Launcher.Core.Services;

public interface IBackendChecker
{
    bool IsBackendInactive();
    Task<bool> CheckBackendStatus();
    Task UpdateBackendStatus();
}
