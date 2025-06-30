using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Gml.Client;
using Gml.Launcher.Assets;
using Gml.Launcher.Core.Exceptions;
using Gml.Launcher.Core.Services;
using Sentry;
using Splat;

namespace Gml.Launcher.Core.Services;

public class BackendChecker: IBackendChecker
{
    private readonly IGmlClientManager _manager;
    public IGmlClientManager Manager => _manager;
    public bool IsOffline => GlobalVariables.BackendInactive;

    public BackendChecker(
        IGmlClientManager? manager=null)
    {
        _manager = manager ?? Locator.Current.GetService<IGmlClientManager>()
            ?? throw new ServiceNotFoundException(typeof(IGmlClientManager));
    }

    public async Task UpdateBackendStatus()
    {
        GlobalVariables.BackendInactive = await BackendIsActive() == false;
    }

    public Task<bool> BackendIsActive()
    {
        try
        {
            return GmlClientManager.CheckAPI(ResourceKeysDictionary.Host);
        }
        catch (TaskCanceledException exception)
        {
            SentrySdk.CaptureException(exception);
            Console.WriteLine(exception);
            return Task.FromResult(false);
        }
        catch (HttpRequestException exception)
        {
            SentrySdk.CaptureException(exception);
            Console.WriteLine(exception);
            return Task.FromResult(false);
        }
    }
}

