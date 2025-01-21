using System;
using Gml.Launcher.ViewModels.Pages;
using Gml.Launcher.Views.Pages;
using ReactiveUI;

namespace Gml.Launcher.Core.Helpers;

public class AppViewLocator : IViewLocator
{
    public IViewFor ResolveView<T>(T? viewModel, string? contract = null)
    {
        return viewModel switch
        {
            ModsPageViewModel context => new ModsPageView { DataContext = context },
            OverviewPageViewModel context => new OverviewPageView { DataContext = context },
            ProfilePageViewModel context => new ProfilePageView { DataContext = context },
            SettingsPageViewModel context => new SettingsPageView { DataContext = context },
            LoginPageViewModel context => new LoginPageView { DataContext = context },

            _ => throw new ArgumentOutOfRangeException(nameof(viewModel))
        };
    }
}
