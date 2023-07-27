using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GamerVII.Launcher.ViewModels.Base;
using System;

namespace GamerVII.Launcher
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object? data)
        {
            
            var name = data?.GetType().FullName!.Replace("ViewModel", "View");
            
            var componentName = data?.GetType().FullName!
                .Replace("ViewModel", "View")
                .Replace("Views", "Views.Components");
            
            var pageName = data?.GetType().FullName!
                .Replace("ViewModel", "View")
                .Replace("Views", "Views.Pages");
            
            var type = Type.GetType(name);

            if (type == null) 
                type = Type.GetType(componentName);
            
            if (type == null) 
                type = Type.GetType(pageName);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            
            
            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}