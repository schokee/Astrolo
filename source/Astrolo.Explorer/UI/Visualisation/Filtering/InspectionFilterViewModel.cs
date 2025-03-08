using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Astrolo.Explorer.Components;
using Astrolo.YiJing;

namespace Astrolo.Explorer.UI.Visualisation.Filtering;

public sealed class InspectionFilterViewModel : Selectable, IDisposable, IHexagramFilter
{
    private readonly IDisposable _shutdown;

    public InspectionFilterViewModel()
    {
        Filters = new(HexagramFilter.All);

        GroupedFilters = CollectionViewSource.GetDefaultView(Filters);
        GroupedFilters.GroupDescriptions.Add(new PropertyGroupDescription(nameof(HexagramFilter.Category)));

        _shutdown = Filters
            .ObserveSelectionChanges()
            .ObserveOn(Scheduler.Default)
            .Throttle(TimeSpan.FromMilliseconds(200))
            .ObserveOn(SynchronizationContext.Current)
            .Subscribe(_ => OnSelectionChanged());
    }

    public void Dispose()
    {
        _shutdown.Dispose();
    }

    public ICollectionView GroupedFilters { get; }

    public PickList<HexagramFilter> Filters { get; }

    public bool Includes(HexagramFigure hexagram)
    {
        return !IsSelected || Filters.CurrentSelection.Any(filter => filter.Includes(hexagram));
    }

    public VisualStates GetState(HexagramFigure hexagram)
    {
        return IsSelected
            ? Filters.CurrentSelection.Aggregate(VisualStates.None, (result, x) => result | x.GetState(hexagram))
            : VisualStates.None;
    }
}
