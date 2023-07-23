using System.Windows;
using System.Windows.Controls;

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





        public string LoadingFile
        {
            get { return (string)GetValue(LoadingFileProperty); }
            set { SetValue(LoadingFileProperty, value); }
        }

        public static readonly DependencyProperty LoadingFileProperty =
            DependencyProperty.Register("LoadingFile", typeof(string), typeof(ServerLoaderComponent), new PropertyMetadata(string.Empty));






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
