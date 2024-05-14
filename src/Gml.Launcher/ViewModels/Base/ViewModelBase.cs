using System.Reactive.Subjects;
using GamerVII.Notification.Avalonia;
using ReactiveUI;

namespace Gml.Launcher.ViewModels.Base;

public class ViewModelBase : ReactiveObject
{

    protected internal Subject<bool> OnClosed = new();

}
