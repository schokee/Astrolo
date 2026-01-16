using System.Reactive.Disposables;
using System.Reactive.Linq;
using Astrolo.HumanDesign;
using Astrolo.HumanDesign.Charting;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace Astrolo.Explorer.UI.Profiling;

public sealed class ComparerViewModel : Screen, IDisposable
{
    private readonly SerialDisposable _shutdown = new();

    public ComparerViewModel(IGateDictionary gateDictionary, ComparisonSubject lhs, ComparisonSubject rhs)
    {
        DisplayName = "Compare";
        Lhs = lhs;
        Rhs = rhs;

        _shutdown.Disposable = Subjects
            .Select(x => Observable.FromEventPattern(h => x.ProfileChanged += h, h => x.ProfileChanged -= h))
            .Merge()
            .Select(_ => lhs.HasContent && rhs.HasContent && lhs.Profile != rhs.Profile
                ? RelationshipChart.Create(gateDictionary, lhs.Chart, rhs.Chart)
                : null)
            .Subscribe(chart => Result = chart);

        foreach (var pair in Subjects.Zip(lhs.SortedProfiles.Cast<INamedProfile>(), (side, profile) => (Side: side, Profile: profile)))
        {
            pair.Side.Profile = pair.Profile;
        }
    }

    public void Dispose()
    {
        _shutdown.Dispose();
    }

    public ComparisonSubject Lhs { get; }

    public ComparisonSubject Rhs { get; }

    public RelationshipChart Result
    {
        get;
        private set => Set(ref field, value);
    }

    [UsedImplicitly]
    public void ClearAll()
    {
        Lhs.Profile = null;
        Rhs.Profile = null;
    }

    private IEnumerable<ComparisonSubject> Subjects => [Lhs, Rhs];
}
