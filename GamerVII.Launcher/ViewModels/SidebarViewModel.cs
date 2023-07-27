using System.Windows.Input;
using GamerVII.Launcher.ViewModels.Base;

namespace GamerVII.Launcher.ViewModels;

/// <summary>
/// View model class for the sidebar, derived from ViewModelBase.
/// </summary>
public class SidebarViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the view model for the list of game servers.
    /// </summary>
    public ServersListViewModel ServersListViewModel { get; }

    /// <summary>
    /// Command to open the profile page.
    /// </summary>
    public ICommand OpenProfilePageCommand { get; set; }

    /// <summary>
    /// Command to execute the logout operation.
    /// </summary>
    public ICommand LogoutCommand { get; set; }

    /// <summary>
    /// Initializes a new instance of the SidebarViewModel class.
    /// </summary>
    public SidebarViewModel()
    {
        // Initialize the ServersListViewModel to handle game servers data.
        ServersListViewModel = new ServersListViewModel();
    }
}