using System;
using Avalonia;

namespace Gml.Launcher.Core.Services;

public class ResourceLocalizationService : ILocalizationService
{
    public string GetString(string key)
    {
        if (Application.Current == null)
        {
            throw new ArgumentException($"Key '{key}' not found in resources");
        }

        return TryGetLocalizedString(key) ?? throw new Exception($"Resource \"{key}\" not found");
    }

    private static string? TryGetLocalizedString(string key)
    {
        return Application.Current!.Resources.TryGetResource(key, null, out var value)
            ? value?.ToString()
            : null;
    }
}
