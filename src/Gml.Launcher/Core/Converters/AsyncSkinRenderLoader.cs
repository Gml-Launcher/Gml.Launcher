using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Gml.Client;
using Gml.Launcher.Core.Services;
using Sentry;

namespace Gml.Launcher.Core.Converters;

public class AsyncSkinRenderLoader
{
    public static readonly AttachedProperty<string> SourceProperty =
        AvaloniaProperty.RegisterAttached<AsyncSkinRenderLoader, Image, string>("Source");

    private static readonly AttachedProperty<bool> IsLoadingProperty =
        AvaloniaProperty.RegisterAttached<AsyncSkinRenderLoader, Image, bool>("IsLoading");

    private static readonly ConcurrentDictionary<Image, CancellationTokenSource> PendingOperations = new();

    static AsyncSkinRenderLoader()
    {
        SourceProperty.Changed.AddClassHandler<Image>(OnSourceChanged);
    }

    private static void OnSourceChanged(Image sender, AvaloniaPropertyChangedEventArgs args)
    {
        Task.Run(() => TryLoadImage(sender, args, 1));
    }

    private static async Task TryLoadImage(Image sender, AvaloniaPropertyChangedEventArgs args, int attempt)
    {
        SetIsLoading(sender, true);
        var cts = PendingOperations.AddOrUpdate(
            sender,
            new CancellationTokenSource(),
            (_, y) =>
            {
                y.Cancel();
                return new CancellationTokenSource();
            });

        var url = args.GetNewValue<string>();

        try
        {
            if (string.IsNullOrEmpty(url))
            {
                PendingOperations.Remove(sender, out _);
                sender.Source = null;
                return;
            }

            if (string.IsNullOrEmpty(url) || !ValidateUrl(url))
                throw new Exception($"User skin not found for user. Url: {url}");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                $"Gml.Launcher-Client-{nameof(GmlClientManager)}/1.0 (OS: {Environment.OSVersion};)");
            var response = await client.GetByteArrayAsync(url, cts.Token);
            using var stream = new MemoryStream(response);

            var bitmap = new Bitmap(new MemoryStream(SkinViewer.GetFront(stream, 128)));

            Dispatcher.UIThread.Invoke(() =>
            {
                if (!cts.Token.IsCancellationRequested) sender.Source = bitmap;
            });
        }
        catch (HttpRequestException exception)
        {
            Debug.WriteLine($"Texture load attempt: {attempt}, reason: {exception.Message}, {url}");
            if (attempt < 3)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                ++attempt;
                await TryLoadImage(sender, args, attempt);
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            SentrySdk.CaptureException(exception);
        }
        finally
        {
            PendingOperations.Remove(sender, out _);
            SetIsLoading(sender, false);
        }
    }

    private static bool ValidateUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public static void SetSource(Image obj, string value)
    {
        obj.SetValue(SourceProperty, value);
    }

    public static string GetSource(Image obj)
    {
        return obj.GetValue(SourceProperty);
    }

    private static void SetIsLoading(Image obj, bool value)
    {
        Dispatcher.UIThread.Invoke(() => { obj.SetValue(IsLoadingProperty, value); });
    }

    public static bool GetIsLoading(Image obj)
    {
        return obj.GetValue(IsLoadingProperty);
    }
}
