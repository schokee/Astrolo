using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Astrolo.Explorer.Messages;
using Astrolo.Presentation.Core.Components;
using Astrolo.YiJing;
using Caliburn.Micro;

namespace Astrolo.Explorer.UI.Visualisation.Filtering
{
    public sealed class InspectionFilterViewModel : Selectable, IDisposable, IHexagramFilter, IHandle<ActiveProfileChanged>
    {
        private readonly IDisposable _shutdown;

        public InspectionFilterViewModel()
        {
            Filters = new PickList<HexagramFilter>(HexagramFilter.All.Prepend(ProfileFilter));

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

        public ProfileFilter ProfileFilter { get; } = new();

        public ICollectionView GroupedFilters { get; }

        public PickList<HexagramFilter> Filters { get; }

        public bool Includes(HexagramFigure hexagram)
        {
            return !IsSelected || Filters.CurrentSelection.Any(filter => filter.Includes(hexagram));
        }

        Task IHandle<ActiveProfileChanged>.HandleAsync(ActiveProfileChanged message, CancellationToken cancellationToken)
        {
            ProfileFilter.Profile = message.GeneKeyProfile;

            if (ProfileFilter.IsSelected)
            {
                OnSelectionChanged();
            }

            return Task.CompletedTask;
        }
    }
}
