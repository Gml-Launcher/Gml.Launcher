using System.Windows.Input;
using GamerVII.Launcher.ViewModels.Base;

namespace GamerVII.Launcher.ViewModels;

/// <summary>
/// View model class for the profile page, derived from PageViewModelBase.
/// </summary>
public class ProfilePageViewModel : PageViewModelBase
{
    /// <summary>
    /// Command to navigate to the main page.
    /// </summary>
    public ICommand GoToMainPageCommand { get; set; }

    /// <summary>
    /// Initializes a new instance of the ProfilePageViewModel class.
    /// </summary>
    public ProfilePageViewModel()
    {
        // Add any initialization logic or data loading here if required.
    }
}