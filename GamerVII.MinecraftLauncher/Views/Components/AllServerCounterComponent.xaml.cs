using System.Windows;
using System.Windows.Controls;

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
