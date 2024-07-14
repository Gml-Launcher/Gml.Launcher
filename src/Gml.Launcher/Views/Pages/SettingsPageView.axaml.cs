using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
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
        if (sender is TextBox textBox)
        {
            textBox.Text = string.Concat(textBox.Text?.Where(char.IsDigit) ?? string.Empty);
        }

    }

    private void OpenFileDialog(object? sender, RoutedEventArgs e)
    {
        
    }
}
