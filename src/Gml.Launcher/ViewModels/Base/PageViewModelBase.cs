using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using GamerVII.Notification.Avalonia;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using ReactiveUI;
using Sentry;
using Splat;

namespace Gml.Launcher.ViewModels.Base;

public class PageViewModelBase : ViewModelBase, IRoutableViewModel
{
    internal readonly ILocalizationService LocalizationService;

    protected PageViewModelBase(IScreen screen, ILocalizationService? localizationService = null)
    {
        LocalizationService = localizationService
                              ?? Locator.Current.GetService<ILocalizationService>()
                              ?? throw new ServiceNotFoundException(typeof(ILocalizationService));

        OpenLinkCommand = ReactiveCommand.Create<string>(OpenLink);
        GoBackCommand = screen.Router.NavigateBack;

        HostScreen = screen;
    }

    public string Title => LocalizationService.GetString(ResourceKeysDictionary.DefaultPageTitle);

    public ICommand OpenLinkCommand { get; }
    public ICommand GoBackCommand { get; set; }
    public IScreen HostScreen { get; }
    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);

    private void OpenLink(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    protected void ShowError(string title, string content)
    {
        if (HostScreen is MainWindowViewModel mainViewModel)
            Dispatcher.UIThread.Invoke(() =>
            {
                mainViewModel.Manager
                    .CreateMessage(true, "#D03E3E",
                        LocalizationService.GetString(title),
                        content)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(3))
                    .Queue();
            });
    }

    protected void ShowSuccess(string title, string content)
    {
        if (HostScreen is MainWindowViewModel mainViewModel)
            Dispatcher.UIThread.Invoke(() =>
            {
                mainViewModel.Manager
                    .CreateMessage(true, "#008C45",
                        LocalizationService.GetString(title),
                        content)
                    .Dismiss()
                    .WithDelay(TimeSpan.FromSeconds(3))
                    .Queue();
            });
    }

    protected Task ExecuteFromNewThread(Func<Task> func)
    {
        var tcs = new TaskCompletionSource<object?>();

        async void RunThreadTask()
        {
            try
            {
                await func();
                tcs.SetResult(null);
            }
            catch (Exception exception)
            {
                tcs.SetException(exception);
                SentrySdk.CaptureException(exception);
            }
        }

        new Thread(RunThreadTask).Start();

        return tcs.Task;
    }
}
