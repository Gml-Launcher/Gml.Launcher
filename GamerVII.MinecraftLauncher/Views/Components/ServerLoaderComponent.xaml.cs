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
    /// Interaction logic for ServerLoaderComponent.xaml
    /// </summary>
    public partial class ServerLoaderComponent : UserControl
    {



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ServerLoaderComponent), new PropertyMetadata(string.Empty));



        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ServerLoaderComponent), new PropertyMetadata(string.Empty));




        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ServerLoaderComponent), new PropertyMetadata(null));





        public ServerLoaderComponent()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
