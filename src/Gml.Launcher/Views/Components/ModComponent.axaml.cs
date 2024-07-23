using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Windows.Input;

namespace Gml.Launcher.Views.Components;

public class ModComponent : TemplatedControl
{
    public static readonly StyledProperty<string> ModNameProperty = AvaloniaProperty.Register<ModComponent, string>(
        nameof(ModName), "3еden Enchanced");
    public static readonly StyledProperty<string> DecsProperty = AvaloniaProperty.Register<ModComponent, string>(
        nameof(Decs), "Пропы/Текстуры ЗВ");
    public static readonly StyledProperty<ICommand> CommandProperty = AvaloniaProperty.Register<ModComponent, ICommand>(
        nameof(Command));
    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<ModComponent, object?>(
        nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public string ModName
    {
        get => GetValue(ModNameProperty);
        set => SetValue(ModNameProperty, value);
    }
    public string Decs
    {
        get => GetValue(DecsProperty);
        set => SetValue(DecsProperty, value);
    }
}

