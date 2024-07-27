using System.Reactive.Subjects;
using ReactiveUI;

namespace Gml.Launcher.ViewModels.Base;

public class ViewModelBase : ReactiveObject
{
    protected internal readonly Subject<bool> OnClosed = new();
}
