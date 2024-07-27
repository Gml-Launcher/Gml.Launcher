using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using Gml.Launcher.ViewModels.Pages;
using ReactiveUI;

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
        if (this.GetVisualRoot() is MainWindow mainWindow)
        {
            var folders = await mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Select a folder"
            });

            if (folders.Count != 1) return;

            ViewModel!.InstallationFolder = folders[0].Path.AbsolutePath;
            ViewModel!.ChangeFolder();
        }
    }
}
