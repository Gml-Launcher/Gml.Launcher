using System.Windows.Input;
using GamerVII.Launcher.ViewModels.Base;

namespace GamerVII.Launcher.ViewModels;

public class ProfilePageViewModel : PageViewModelBase
{

    public ICommand GoTaMainPageCommand { get; set; }

    public ProfilePageViewModel()
    {
        
    }

}