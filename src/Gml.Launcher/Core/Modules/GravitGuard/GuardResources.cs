using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gml.Launcher.Core.Modules.GravitGuard;

public static class GuardResources
{
    public static string ExtractGuardFiles()
    {
        var arch = Environment.Is64BitProcess ? "x64" : "x86";
        var tempPath = Path.Combine(Path.GetTempPath(),
            $"Guard_{Guid.NewGuid():N}", arch);

        Directory.CreateDirectory(tempPath);

        // Извлекаем файлы из ресурсов с учетом архитектуры
        ExtractResource($"Guard.{arch}.GuardDLL.dll",
            Path.Combine(tempPath, "GuardDLL.dll"));

        ExtractResource($"Guard.{arch}.GravitGuard2.exe",
            Path.Combine(tempPath, "GravitGuard2.exe"));

        return tempPath;
    }

    private static void ExtractResource(string resourceName, string targetPath)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
                throw new FileNotFoundException($"Resource not found: {resourceName}");

            using (var fileStream = new FileStream(targetPath, FileMode.Create))
            {
                stream.CopyTo(fileStream);
            }
        }
    }
    public static void CleanupGuardFiles()
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "GuardFiles");
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки очистки
            Debug.WriteLine($"Failed to cleanup guard files: {ex.Message}");
        }
    }
}
