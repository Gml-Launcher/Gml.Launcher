using GamerVII.MinecraftLauncher.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GamerVII.MinecraftLauncher.Views.Components
{
    /// <summary>
    /// Interaction logic for ServersListComponent.xaml
    /// </summary>
    public partial class ServersListComponent : UserControl
    {

        public ObservableCollection<IServer> ServersList
        {
            get { return (ObservableCollection<IServer>)GetValue(ServersListProperty); }
            set { SetValue(ServersListProperty, value); }
        }

        public static readonly DependencyProperty ServersListProperty =
            DependencyProperty.Register("ServersList", typeof(ObservableCollection<IServer>), typeof(ServersListComponent), new PropertyMetadata(null));




        public IServer SelectedServer
        {
            get { return (IServer)GetValue(SelectedServerProperty); }
            set { SetValue(SelectedServerProperty, value); }
        }

        public static readonly DependencyProperty SelectedServerProperty =
            DependencyProperty.Register("SelectedServer", typeof(IServer), typeof(ServersListComponent), new PropertyMetadata(null));



        public ServersListComponent()
        {
            InitializeComponent();
        }
    }
}
