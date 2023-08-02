using System.Windows.Input;
using GamerVII.Launcher.ViewModels.Base;
using ReactiveUI;

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
    
    
    public int MemorySize
    {
        get => _memorySize;
        set => this.RaiseAndSetIfChanged(ref _memorySize, value);
    }
    public int WindowWidth
    {
        get => _windowWidth;
        set => this.RaiseAndSetIfChanged(ref _windowWidth, value);
    }
    public int WindowHeight
    {
        get => _windowHeight;
        set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
    }
    
    public bool IsFullScreen
    {
        get => _isFullScreen;
        set => this.RaiseAndSetIfChanged(ref _isFullScreen, value);
    }
    
    private int _memorySize = 1024;
    private int _windowWidth = 900;
    private int _windowHeight = 600;
    private bool _isFullScreen = false;
}
