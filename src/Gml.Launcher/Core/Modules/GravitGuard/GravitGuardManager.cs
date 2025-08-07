using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Gml.Launcher.Core.Modules.GravitGuard;

public class GravitGuardManager
{
    private static string _guardPath;


    public static void InitializeGuard()
    {
        try
        {
            // Извлекаем файлы из ресурсов во временную директорию
            _guardPath = GuardResources.ExtractGuardFiles();

            var exePath = Path.Combine(_guardPath, "GravitGuard2.exe");
            var dllPath = Path.Combine(_guardPath, "GuardDLL.dll");

            // Загружаем DLL и запускаем защиту
            LoadGuardDll(dllPath);
            StartGuardProcess(exePath);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to initialize guard: {ex.Message}");
        }
    }

    public static void Cleanup()
    {
        GuardResources.CleanupGuardFiles();
    }

    private static void LoadGuardDll(string dllPath)
    {
        try
        {
            NativeLibrary.Load(dllPath);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to load guard DLL: {ex.Message}");
        }
    }

    private static void StartGuardProcess(string exePath)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process guardProcess = Process.Start(startInfo);

            // Добавляем обработчик завершения процесса
            guardProcess.EnableRaisingEvents = true;
            guardProcess.Exited += (sender, args) =>
            {
                // Обработка завершения процесса защиты
                Console.WriteLine("Guard process exited");
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to start guard process: {ex.Message}");
        }
    }

}
