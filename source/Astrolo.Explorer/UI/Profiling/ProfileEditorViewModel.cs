using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Astrolo.Explorer.UI.Visualisation.Mandala;
using Astrolo.GeneKeys;
using Astrolo.HumanDesign;
using Astrolo.HumanDesign.Charting;
using Astrolo.YiJing;
using Caliburn.Micro;
using CommunityToolkit.Mvvm.Input;
using JetBrains.Annotations;
using MoreLinq;

namespace Astrolo.Explorer.UI.Profiling;

public sealed class ProfileEditorViewModel : Screen, IDisposable
{
    private const string Initial =
        "15.5;10.5;16.6;42.3;32.3;35.1;62.1;16.5;60.5;59.2;15.3;48.4;7.2;" +
        "46.3;25.3;32.3;51.2;57.2;57.4;44.4;33.4;61.2;64.1;52.2;48.6;7.5";

    private readonly SerialDisposable _shutdown = new();
    private readonly IGateDictionary _gateDictionary;
    private readonly KeynoteMap _keyNotes = new();

    private PersonalChart _currentChart;
    private IGeneKey _selectedGeneKey;
    private ChartValue _selectedChartValue;

    public ProfileEditorViewModel(GeneKeyTable geneKeys, IGateDictionary gateDictionary)
    {
        DisplayName = "Profile";

        GeneKeys = geneKeys;
        Editor = new ProfileEditor(n => geneKeys[n]);

        MandalaFigures = geneKeys
            .Select(x => new MandalaFigure(x))
            .OrderBy(x => x.StartAngle)
            .ToList();

        _gateDictionary = gateDictionary;
        _shutdown.Disposable = Observable
            .FromEvent(h => Editor.ProfileChanged += h, h => Editor.ProfileChanged -= h)
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Select(_ => CreateChart())
            .Subscribe(x => CurrentChart = x);

        Editor.InitializeFrom(Initial);

        SelectPointCommand = new RelayCommand<object>(SelectPoint, o => o != null);
        NavigateToGeneKeyCommand = new RelayCommand<object>(n => SelectedGeneKey = GeneKeys[(int)n], n => n is int);
        NavigateToGeneKeyDetailCommand = new RelayCommand(() => SelectedGeneKey.InfoLink.Browse());

    }

    public void Dispose()
    {
        _shutdown.Dispose();
    }

    #region Commands

    public ICommand SelectPointCommand { get; }

    public ICommand NavigateToProgrammingPartnerCommand => new RelayCommand(() => SelectedGeneKey = SelectedGeneKey.ProgrammingPartner, () => SelectedGeneKey?.ProgrammingPartner is not null);

    public ICommand NavigateToGeneKeyCommand { get; }

    public ICommand NavigateToGeneKeyDetailCommand { get; }

    #endregion

    public ProfileEditor Editor { get; }

    public GeneKeyTable GeneKeys { get; }

    public IReadOnlyList<MandalaFigure> MandalaFigures { get; }

    public PersonalChart FilteredChart
    {
        get
        {
            var personalChart = CurrentChart;

            return personalChart is null || ViewPersonalityAndDesign
                ? personalChart
                : personalChart.Filter(ViewDesignOnly);
        }
    }

    private bool _viewPersonalityAndDesign = true;
    public bool ViewPersonalityAndDesign
    {
        get => _viewPersonalityAndDesign;
        set
        {
            if (Set(ref _viewPersonalityAndDesign, value))
            {
                NotifyOfPropertyChange(nameof(FilteredChart));
            }
        }
    }

    private bool _viewDesignOnly;
    public bool ViewDesignOnly
    {
        get => _viewDesignOnly;
        set
        {
            if (Set(ref _viewDesignOnly, value))
            {
                NotifyOfPropertyChange(nameof(FilteredChart));
            }
        }
    }

    private bool _viewPersonalityOnly;
    public bool ViewPersonalityOnly
    {
        get => _viewPersonalityOnly;
        set
        {
            if (Set(ref _viewPersonalityOnly, value))
            {
                NotifyOfPropertyChange(nameof(FilteredChart));
            }
        }
    }

    private bool _showStarPearl;
    public bool ShowStarPearl
    {
        get => _showStarPearl;
        set
        {
            if (Set(ref _showStarPearl, value))
            {
                NotifyOfPropertyChange(nameof(Points));
                UpdateSelectedFigures();
            }
        }
    }

    [UsedImplicitly]
    public double StartAngle => MandalaFigures[0].GeneKey.Gate.MandalaSlice.StartAngle;

    public IEnumerable<GeneKeyPoint> Points
    {
        get
        {
            if (FilteredChart is null)
            {
                return [];
            }

            var points = CurrentChart
                .SelectGeneKeys(CreatePoint)
                .ToList();

             return ShowStarPearl
                ? points.Where(x => x.Sphere != Sphere.SQ)
                : points.TakeUntil(x => x.Sphere == Sphere.Pearl);

             GeneKeyPoint CreatePoint(Sphere sphere, ChartValue chartValue)
             {
                 var gate = chartValue.Gate;
                 var isCenterDefined = CurrentChart.Centers[gate.Center] == CenterState.Defined;

                 return new GeneKeyPoint(GeneKeys[gate.Number], sphere, chartValue, _keyNotes[sphere, chartValue.Line.Number], isCenterDefined);
             }
        }
    }


    public HexagramFigure SelectedHexagram => SelectedGeneKey?.Hexagram;

    public IGateInfo SelectedGate => SelectedGeneKey?.Gate;

    public IGeneKey SelectedGeneKey
    {
        get => _selectedGeneKey;
        set
        {
            if (Set(ref _selectedGeneKey, value))
            {
                NotifyOfPropertyChange(nameof(SelectedHexagram));
                NotifyOfPropertyChange(nameof(SelectedGate));
                NotifyOfPropertyChange(nameof(NavigateToProgrammingPartnerCommand));
            }
        }
    }

    public ChartValue SelectedChartValue
    {
        get => _selectedChartValue;
        set
        {
            if (Set(ref _selectedChartValue, value))
            {
                SelectedGeneKey = SelectedChartValue != null ? GeneKeys[SelectedChartValue.Gate.Number] : null;
            }
        }
    }

    private PersonalChart CurrentChart
    {
        get => _currentChart;
        set
        {
            if (Set(ref _currentChart, value))
            {
                NotifyOfPropertyChange(nameof(FilteredChart));
                NotifyOfPropertyChange(nameof(Points));
                UpdateSelectedFigures();
                SelectedChartValue = CurrentChart?.Values.FirstOrDefault();
            }
        }
    }

    private PersonalChart CreateChart()
    {
        return Editor.IsComplete
            ? PersonalChart.Create(_gateDictionary, Editor.GetValue)
            : null;
    }

    private void SelectPoint(object point)
    {
        switch (point)
        {
            case IGeneKey geneKey:
                SelectedGeneKey = geneKey;
                break;

            case IGateInfo gate:
                SelectedGeneKey = GeneKeys[gate.Number];
                break;

            case HexagramFigure hexagram:
                SelectedGeneKey = GeneKeys[hexagram.Number];
                break;

            case ProfilePoint { Value: not null } profilePoint:
                SelectedGeneKey = GeneKeys[profilePoint.Value.Value.Hexagram];
                break;

            case GeneKeyPoint geneKeyPoint:
                SelectedChartValue = geneKeyPoint.ChartValue;
                break;

            case ChartValue chartValue:
                SelectedChartValue = chartValue;
                break;

            default:
                SelectedChartValue = null;
                break;
        }
    }

    private void UpdateSelectedFigures()
    {
        var allFigures = MandalaFigures.GroupJoin(Points, x => x.GeneKey.Number, x => x.GeneKey.Number, (f, l) => (Figure: f, Select: l.Any()));

        foreach (var pair in allFigures)
        {
            pair.Figure.IsSelected = pair.Select;
        }
    }
}
