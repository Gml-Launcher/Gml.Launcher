using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;

namespace Gml.Launcher.Core.Converters;

public class UrlRedirectProperty
{
    private static readonly AttachedProperty<string> RedirectUrlProperty =
        AvaloniaProperty.RegisterAttached<UrlRedirectProperty, Control, string>("RedirectUrl");

    public UrlRedirectProperty()
    {
        RedirectUrlProperty.Changed.AddClassHandler<Control>(OnSourceChanged);
    }

    public static void SetRedirectUrl(Control obj, string value)
    {
        obj.SetValue(RedirectUrlProperty, value);
    }

    public static string GetRedirectUrl(Control obj)
    {
        return obj.GetValue(RedirectUrlProperty);
    }

    private void OnSourceChanged(Control control, AvaloniaPropertyChangedEventArgs args)
    {
        var url = args.GetNewValue<string>();

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}
