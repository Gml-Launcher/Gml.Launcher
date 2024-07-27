using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;

namespace Gml.Launcher.Core.Converters;

public class UrlRedirectProperty
{
    private static readonly AttachedProperty<string> RedirectUrlProperty =
        AvaloniaProperty.RegisterAttached<UrlRedirectProperty, Control, string>("RedirectUrl");

    public static void SetRedirectUrl(Control obj, string value) => obj.SetValue(RedirectUrlProperty, value);
    public static string GetRedirectUrl(Control obj) => obj.GetValue(RedirectUrlProperty);

    public UrlRedirectProperty()
    {
        RedirectUrlProperty.Changed.AddClassHandler<Control>(OnSourceChanged);
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
