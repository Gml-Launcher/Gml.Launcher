using System.Windows.Input;
using GamerVII.Launcher.ViewModels.Base;

namespace GamerVII.Launcher.ViewModels;

/// <summary>
/// View model class for the client settings page, derived from PageViewModelBase.
/// </summary>
public class ClientSettingsPageViewModel : PageViewModelBase
{
    /// <summary>
    /// Command to navigate to the main page.
    /// </summary>
    public ICommand GoToMainPageCommand { get; set; }
}
