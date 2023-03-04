using GamerVII.MinecraftLauncher.Core.SkinViewer;
using GamerVII.MinecraftLauncher.Core.SkinViewer.Helpers;
using GamerVII.MinecraftLauncher.Models;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GamerVII.MinecraftLauncher.ViewModels
{
    public class DashboardViewModel : MvxViewModel
    {

        #region Текущая страница
        private ObservableCollection<IServer> _serversList = new ObservableCollection<IServer>();
        public ObservableCollection<IServer> ServersList
        {
            get => _serversList;
            set => SetProperty(ref _serversList, value);
        }
        #endregion
        #region Скин пользователя
        private ImageSource _skin;
        public ImageSource Skin
        {
            get => _skin;
            set => SetProperty(ref _skin, value);
        }
        #endregion

        public DashboardViewModel()
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(@"C:\Users\GamerVII\Source\Repos\minecraft-launcher\GamerVII.MinecraftLauncher\Views\Resources\Images\default.png");
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            ServersList.Add(new Server
            {
                Name = "Hitech",
                Image = bitmapImage

            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });

            ServersList.Add(new Server
            {
                Name = "Magic",
                Image = bitmapImage
            });



            LoadSkin();
        }

        private async void LoadSkin()
        {

            await Task.Run(async () =>
            {
                Task.Delay(3000).Wait();
                SkinViewerManager skinViewerManager = new SkinViewerManager("https://ru-minecraft.ru/uploads/posts/2018-01/1516387236_skin_stasicmirza.png");
                //SkinViewerManager skinViewerManager = new SkinViewerManager(@"https://pngimage.net/wp-content/uploads/2018/06/скины-png-64x32-8.png");

                await skinViewerManager.LoadAsync();

                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    Skin = skinViewerManager.GetFront(15);
                });

            });
        
        }
    }
}
