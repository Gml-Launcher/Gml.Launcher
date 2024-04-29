using System;
using System.Diagnostics;
using System.Windows.Input;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using ReactiveUI;
using Splat;

namespace Gml.Launcher.ViewModels.Base;

public class PageViewModelBase : ViewModelBase, IRoutableViewModel
{
    public IScreen HostScreen { get; }
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    protected readonly ILocalizationService LocalizationService;

    public string Title => LocalizationService.GetString(ResourceKeysDictionary.DefaultPageTitle);

    public ICommand OpenLinkCommand { get; }
    public ICommand GoBackCommand { get; set; }

    protected PageViewModelBase(IScreen screen, ILocalizationService? localizationService = null)
    {
        LocalizationService = localizationService
                               ?? Locator.Current.GetService<ILocalizationService>()
                               ?? throw new ServiceNotFoundException(typeof(ILocalizationService));

        OpenLinkCommand = ReactiveCommand.Create<string>(OpenLink);
        GoBackCommand = screen.Router.NavigateBack;

        HostScreen = screen;
    }

    private void OpenLink(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

}
