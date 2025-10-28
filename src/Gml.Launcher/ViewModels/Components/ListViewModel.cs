using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using Gml.Dto.Profile;
using Gml.Launcher.ViewModels.Base;
using ReactiveUI;

namespace Gml.Launcher.ViewModels.Components;

public class ListViewModel : ViewModelBase
{
    internal readonly Subject<ProfileReadDto?> ProfileChanged = new();


    private ObservableCollection<ProfileReadDto>? _profiles;
    private ProfileReadDto? _selectedProfile;

    public ObservableCollection<ProfileReadDto>? Profiles
    {
        get => _profiles;
        set
        {
            this.RaiseAndSetIfChanged(ref _profiles, value);
            this.RaisePropertyChanged(nameof(IsNotLoaded));
            this.RaisePropertyChanged(nameof(HasItems));
            this.RaisePropertyChanged(nameof(HasSelectedItem));
        }
    }

    public ProfileReadDto? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedProfile, value);
            this.RaisePropertyChanged(nameof(HasSelectedItem));

            ProfileChanged.OnNext(value);
        }
    }

    public bool IsNotLoaded => _profiles == null;
    public bool HasItems => _profiles != null && _profiles.Any();
    public bool HasSelectedItem => _selectedProfile != null;
}
