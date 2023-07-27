using System.Windows.Input;
using GamerVII.Launcher.ViewModels.Base;

namespace GamerVII.Launcher.ViewModels;

public class SidebarViewModel : ViewModelBase
{

    public ServersListViewModel ServersListViewModel { get;}

    public ICommand OpenProfilePageCommand { get; set; }
    public ICommand LogoutCommand { get; set; }
    
    public SidebarViewModel()
    {
        ServersListViewModel = new ServersListViewModel();
    }

}
