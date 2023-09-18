using System.Threading;
using System.Threading.Tasks;
using Astrolo.Explorer.UI.Visualisation.Filtering;
using Astrolo.GeneKeys;
using Astrolo.HumanDesign;
using Astrolo.HumanDesign.Charting;
using Caliburn.Micro;
using JetBrains.Annotations;

namespace Astrolo.Explorer.UI.Visualisation.Mandala
{
    public sealed class MandalaViewModel : Screen, ISupportFiltering
    {
        private bool _showBinary;
        private bool _isFilterActive;

        public MandalaViewModel(GeneKeyTable geneKeys)
        {
            DisplayName = "Mandala";

            GeneKeys = geneKeys
                .Select(x => new MandalaFigure(x))
                .OrderBy(x => x.StartAngle)
                .ToList();

            StartAngle = GeneKeys[0].Hexagram.StartAngle();

            var lookup = GeneKeys.ToDictionary(x => x.Hexagram.Number);

            Chart = new MandalaChart(geneKeys.Gates, gate => lookup[gate.Number]);
        }

        public double StartAngle { get; }

        public IReadOnlyList<MandalaFigure> GeneKeys { get; }

        public Chart Chart { get; }

        public bool IsFilterActive
        {
            get => _isFilterActive;
            set
            {
                if (value == _isFilterActive)
                    return;
                _isFilterActive = value;
                NotifyOfPropertyChange();
            }
        }

        [UsedImplicitly]
        public bool ShowBinary
        {
            get => _showBinary;
            set
            {
                if (value == _showBinary)
                    return;
                _showBinary = value;
                NotifyOfPropertyChange();
            }
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Refresh();
            return base.OnActivateAsync(cancellationToken);
        }

        void ISupportFiltering.UpdateSelection(IHexagramFilter filter)
        {
            foreach (var figure in GeneKeys)
            {
                figure.IsSelected = filter.Includes(figure.Hexagram);
            }

            IsFilterActive = filter.IsSelected;
        }

        private sealed class MandalaChart : Chart
        {
            public MandalaChart(IGateDictionary gateLookup, Func<IGateInfo, IGateConfiguration> createConfig)
                : base(gateLookup, createConfig, _ => false)
            {
            }
        }
    }
}
