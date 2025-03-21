using Astrolo.HumanDesign;
using Astrolo.HumanDesign.Charting;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace Astrolo.Explorer.UI.Profiling;

public sealed class ComparisonSubject : PropertyChangedBase
{
    private INamedProfile _profile;

    private Func<INamedProfile, PersonalChart> CreateChart { get; }

    public event EventHandler ProfileChanged;

    public ComparisonSubject(IProfileDirectory profileDirectory, IGateDictionary gateDictionary)
    {
        SortedProfiles = CollectionViewSource.GetDefaultView(profileDirectory);
        CreateChart = profile => profile is null ? null : PersonalChart.Create(gateDictionary, profile.Editor.GetValue);
    }

    public ICollectionView SortedProfiles { get; }

    public bool HasContent => Profile is not null;

    public INamedProfile Profile
    {
        get => _profile;
        set
        {
            if (Set(ref _profile, value))
            {
                NotifyOfPropertyChange(nameof(HasContent));
                NotifyOfPropertyChange(nameof(Chart));

                ProfileChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public PersonalChart Chart => CreateChart(Profile);

    [UsedImplicitly]
    public void Clear()
    {
        Profile = null;
    }
}
