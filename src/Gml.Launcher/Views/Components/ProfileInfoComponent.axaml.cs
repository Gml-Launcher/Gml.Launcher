using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using Gml.Launcher.Models;

namespace Gml.Launcher.Views.Components;

public class ProfileInfoComponent : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<ProfileInfoItem>> ProfileInfoItemsProperty =
        AvaloniaProperty.Register<ProfileUserControl, IEnumerable<ProfileInfoItem>>(
            nameof(ProfileInfoItems), new List<ProfileInfoItem>
            {
                // new("На проекте", "150 дней"),
                // new("Наиграно", " 551 час 45 мин"),
                // new("Баланс", "150 руб."),
                // new("Группа", "Premium"),
            });


    public IEnumerable<ProfileInfoItem> ProfileInfoItems
    {
        get => GetValue(ProfileInfoItemsProperty);
        set => SetValue(ProfileInfoItemsProperty, value);
    }
}
