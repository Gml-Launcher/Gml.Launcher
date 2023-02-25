using System;
using System.Collections.Generic;
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
    /// Interaction logic for AllServerCounterComponent.xaml
    /// </summary>
    public partial class AllServerCounterComponent : UserControl
    {

        public int PlayerCount
        {
            get { return (int)GetValue(PlayerCountProperty); }
            set { SetValue(PlayerCountProperty, value); }
        }

        public static readonly DependencyProperty PlayerCountProperty =
            DependencyProperty.Register("PlayerCount", typeof(int), typeof(AllServerCounterComponent), new PropertyMetadata(0));

        public AllServerCounterComponent()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
