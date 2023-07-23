using GamerVII.MinecraftLauncher.Core.SkinViewer;
using System.Threading.Tasks;

namespace GamerVII.MinecraftLauncher.Services.Skin;

class SkinService : ISkinService
{
    public async Task<SkinViewerManager> GetSkinViewerManager(string url)
    {
        SkinViewerManager skinViewerManager = new SkinViewerManager(url);

        await skinViewerManager.LoadAsync();

        return skinViewerManager;
    }
}
