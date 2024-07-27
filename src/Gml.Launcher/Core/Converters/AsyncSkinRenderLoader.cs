using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Gml.Client;
using Gml.Launcher.Core.Services;

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

    private static async void OnSourceChanged(Image sender, AvaloniaPropertyChangedEventArgs args)
    {
        try
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

            if (string.IsNullOrEmpty(url))
            {
                PendingOperations.Remove(sender, out _);
                sender.Source = null;
                return;
            }

            if (string.IsNullOrEmpty(url) || !ValidateUrl(url)) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                $"Gml.Launcher-Client-{nameof(GmlClientManager)}/1.0 (OS: {Environment.OSVersion};)");
            var response = await client.GetByteArrayAsync(url, cts.Token);
            using var stream = new MemoryStream(response);

            var bitmap = new Bitmap(new MemoryStream(SkinViewer.GetFront(stream, 128)));

            if (!cts.Token.IsCancellationRequested)
            {
                sender.Source = bitmap;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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

    public static void SetSource(Image obj, string value) => obj.SetValue(SourceProperty, value);
    public static string GetSource(Image obj) => obj.GetValue(SourceProperty);

    private static void SetIsLoading(Image obj, bool value) => obj.SetValue(IsLoadingProperty, value);
    public static bool GetIsLoading(Image obj) => obj.GetValue(IsLoadingProperty);
}
