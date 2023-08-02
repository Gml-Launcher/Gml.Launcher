using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace GamerVII.Launcher.Views.Components;

public class ButtonControl : TemplatedControl
{

    public static readonly StyledProperty<string> IconPathProperty = AvaloniaProperty.Register<ButtonControl, string>(
        nameof(IconPath), "/Assets/document.svg");

    public string IconPath
    {
        get => GetValue(IconPathProperty);
        set => SetValue(IconPathProperty, value);
    }

    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<ButtonControl, string>(
        nameof(Text), "КНОПКА");

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<ButtonControl, ICommand>(
        nameof(Command));

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}