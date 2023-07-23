using GamerVII.MinecraftLauncher.ViewModels.Base;
using GamerVII.MinecraftLauncher.Views.Pages;
using ReactiveUI;
using System.Windows.Controls;

namespace GamerVII.MinecraftLauncher.ViewModels;

public class MainViewModel : BaseViewModel
{

    private Page _dashboardPage = new DashboardPage();

    #region Текущая страница
    private Page _currentPage;
    public Page ContentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }
    #endregion

    public MainViewModel()
    {
        ContentPage = _dashboardPage;
    }

}
