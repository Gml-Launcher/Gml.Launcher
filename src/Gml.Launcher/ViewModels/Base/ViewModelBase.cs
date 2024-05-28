using System;
using System.Reactive.Subjects;
using ReactiveUI;

namespace Gml.Launcher.ViewModels.Base;

public class ViewModelBase : ReactiveObject
{
    protected internal Subject<bool> OnClosed = new();
}
