using Avalonia.Controls;
using Avalonia.Input;

namespace GamerVII.Launcher.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
        }
    }
}