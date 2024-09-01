using System;
using System.Linq;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using GamerVII.Notification.Avalonia;
using Gml.Launcher.Assets;
using Gml.Launcher.ViewModels.Pages;
using ReactiveUI;
// using Sentry;

namespace Gml.Launcher.Views.Pages;

public partial class SettingsPageView : ReactiveUserControl<SettingsPageViewModel>
{
    public SettingsPageView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    private void OnTextInput(object sender, AvaloniaPropertyChangedEventArgs e)
    {
    }


    private void OnTextInput(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox) textBox.Text = string.Concat(textBox.Text?.Where(char.IsDigit) ?? string.Empty);
    }

    private async void OpenFileDialog(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
            {
                var folders = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    AllowMultiple = false,
                    Title = "Select a folder"
                });

                if (folders.Count != 1) return;

                ViewModel!.InstallationFolder = Path.GetFullPath(folders[0].Path.AbsolutePath);
                ViewModel!.ChangeFolder();
            }
        }
        catch (Exception exception)
        {
            // Log the exception details to Sentry
            // SentrySdk.CaptureException(exception);
            // TODO Sentry send

            // Existing log statement
            Console.WriteLine(exception.ToString());

            // Show error notification
            ViewModel?.MainViewModel.Manager
                .CreateMessage(true, "#D03E3E",
                ViewModel.LocalizationService.GetString(ResourceKeysDictionary.Error),
                ViewModel.LocalizationService.GetString(ResourceKeysDictionary.InvalidFolder))
                .Dismiss()
                .WithDelay(TimeSpan.FromSeconds(3))
                .Queue();
        }
    }
}
