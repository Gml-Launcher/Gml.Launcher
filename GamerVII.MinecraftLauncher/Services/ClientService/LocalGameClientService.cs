using GamerVII.MinecraftLauncher.Models.Client;
using GamerVII.MinecraftLauncher.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GamerVII.MinecraftLauncher.Services.ClientService;

internal class LocalGameClientService : IGameClientService
{
    public async Task<IEnumerable<IGameClient>> GetClientsAsync()
    {

        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(@"C:\Users\GamerVII\Source\Repos\minecraft-launcher\GamerVII.MinecraftLauncher\Views\Resources\Images\default.png");
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();

        List<GameClient> servers = new List<GameClient>();

        for (int i = 0; i < 15; i++)
        {
            servers.Add(new GameClient
            {
                Name = $"Сервер-{i}",
                Image = bitmapImage,
                Description = "Просто проект майнкрафт, без модов. Не знаю что ты тут ищешь",
                Version = "1.7.10",
                ModLoaderType = ModLoaderType.Forge
            });
        }

        return await Task.FromResult(servers);
    }
}
