using GamerVII.MinecraftLauncher.Core.SkinViewer;
using GamerVII.MinecraftLauncher.Services.Base;
using System.Threading.Tasks;

namespace GamerVII.MinecraftLauncher.Services.Skin;

public interface ISkinService : IServiceBase
{

    Task<SkinViewerManager> GetSkinViewerManager(string url);

}
