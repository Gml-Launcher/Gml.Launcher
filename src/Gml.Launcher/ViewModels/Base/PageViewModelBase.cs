using System;
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

    protected PageViewModelBase(IScreen screen, ILocalizationService? localizationService = null)
    {
        LocalizationService = localizationService
                               ?? Locator.Current.GetService<ILocalizationService>()
                               ?? throw new ServiceNotFoundException(typeof(ILocalizationService));

        HostScreen = screen;
    }

}
