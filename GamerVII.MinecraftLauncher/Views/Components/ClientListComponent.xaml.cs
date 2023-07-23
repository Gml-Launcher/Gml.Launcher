using GamerVII.MinecraftLauncher.Models.Client;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace GamerVII.MinecraftLauncher.Views.Components
{
    public partial class ClientListComponent : UserControl
    {

        public ObservableCollection<IGameClient> GameClients
        {
            get { return (ObservableCollection<IGameClient>)GetValue(GameClientsProperty); }
            set { SetValue(GameClientsProperty, value); }
        }

        public static readonly DependencyProperty GameClientsProperty =
            DependencyProperty.Register("GameClients", typeof(ObservableCollection<IGameClient>), typeof(ClientListComponent), new PropertyMetadata(null));




        public IGameClient SelectedClient
        {
            get { return (IGameClient)GetValue(SelectedClientProperty); }
            set { SetValue(SelectedClientProperty, value); }
        }

        public static readonly DependencyProperty SelectedClientProperty =
            DependencyProperty.Register("SelectedClient", typeof(IGameClient), typeof(ClientListComponent), new PropertyMetadata(null));



        public ClientListComponent()
        {
            InitializeComponent();
        }
    }
}
