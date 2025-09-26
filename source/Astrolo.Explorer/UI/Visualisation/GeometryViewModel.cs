using System.Threading;
using System.Threading.Tasks;
using Astrolo.Explorer.UI.Visualisation.Filtering;
using Astrolo.Explorer.UI.Visualisation.Hive;
using Astrolo.Explorer.UI.Visualisation.Mandala;
using Astrolo.Explorer.UI.Visualisation.Sphere;
using Astrolo.Explorer.UI.Visualisation.Table;
using Caliburn.Micro;
using MoreLinq;

namespace Astrolo.Explorer.UI.Visualisation;

public sealed class GeometryViewModel : Conductor<Screen>.Collection.OneActive
{
    private bool _showSphere;

    public GeometryViewModel(
        YiSphereViewModel sphere,
        HexChartViewModel hexChart,
        TabularViewModel table,
        MandalaViewModel mandala,
        InspectionFilterViewModel filter)
    {
        DisplayName = "Geometry";

        Items.Add(hexChart);
        Items.Add(mandala);
        Items.Add(table);
        Items.Add(sphere);

        ActiveItem = hexChart;

        Sphere = sphere;
        Sphere.Filter = filter;

        HexChart = hexChart;

        Filter = filter;
        Filter.SelectionChanged += delegate { ApplyFilters(); };
    }

    public InspectionFilterViewModel Filter { get; }

    public HexChartViewModel HexChart { get; }

    public YiSphereViewModel Sphere { get; }

    public bool ShowHexChart => !ShowSphere;

    public bool ShowSphere
    {
        get => _showSphere;
        set
        {
            if (value == _showSphere) return;
            _showSphere = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(ShowHexChart));
        }
    }

    protected override Task OnActivatedAsync(CancellationToken cancellationToken)
    {
        ApplyFilters();
        return base.OnActivatedAsync(cancellationToken);
    }

    private void ApplyFilters()
    {
        Items.OfType<ISupportFiltering>().ForEach(x => x.UpdateSelection(Filter));
    }
}
