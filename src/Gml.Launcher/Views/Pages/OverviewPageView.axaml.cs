using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Gml.Launcher.ViewModels.Pages;
using ReactiveUI;

namespace Gml.Launcher.Views.Pages;

public partial class OverviewPageView : ReactiveUserControl<OverviewPageViewModel>
{
    public OverviewPageView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
}
