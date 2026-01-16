using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Astrolo.Explorer.UI.Visualisation.Mandala;
using Astrolo.Explorer.Windows;
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

    private readonly SerialDisposable _shutdown = new();
    private readonly IGateDictionary _gateDictionary;
    private readonly KeynoteMap _keyNotes = new();

    private IUserPrompt Prompt { get; }
    private Func<NameEditorViewModel> CreateNewProfileNameEditor { get; }
    private Func<NameEditorViewModel> CreateRenameEditor { get; }

    public ProfileEditorViewModel(
        IProfileDirectory profileDirectory,
        GeneKeyTable geneKeys,
        IGateDictionary gateDictionary,
        IUserPrompt prompt,
        NameEditorViewModel.Factory createNameEditor)
    {
        _gateDictionary = gateDictionary;

        ProfileDirectory = profileDirectory;
        DisplayName = "Profile";

        GeneKeys = geneKeys;
        Prompt = prompt;
        CreateRenameEditor = () => createNameEditor(profileDirectory.IsNameUnique, CurrentProfile.Name, rename: true);
        CreateNewProfileNameEditor = () => createNameEditor(profileDirectory.IsNameUnique, profileDirectory.GenerateUniqueName("New Profile", x => x.Name));

        MandalaFigures = geneKeys
            .Select(x => new MandalaFigure(x))
            .OrderBy(x => x.StartAngle)
            .ToList();


        SelectPointCommand = new RelayCommand<object>(SelectPoint, o => o != null);
        NavigateToGeneKeyCommand = new RelayCommand<object>(n => SelectedGeneKey = GeneKeys[(int)n], n => n is int);
        NavigateToGeneKeyDetailCommand = new RelayCommand(() => SelectedGeneKey.InfoLink.Browse());

        SortedProfiles = CollectionViewSource.GetDefaultView(profileDirectory);
        SortedProfiles.SortDescriptions.Add(new SortDescription(nameof(INamedProfile.Name), ListSortDirection.Ascending));

        var selectionChanged = Observable
            .FromEventPattern(h => SortedProfiles.CurrentChanged += h, h => SortedProfiles.CurrentChanged -= h)
            .Do(_ =>
            {
                NotifyOfPropertyChange(nameof(CanRename));
                NotifyOfPropertyChange(nameof(CanDelete));
                NotifyOfPropertyChange(nameof(CurrentProfile));
                NotifyOfPropertyChange(nameof(Editor));
                NotifyOfPropertyChange(nameof(FilteredChart));
            })
            .Select(_ => Unit.Default);

        _shutdown.Disposable = Observable
            .FromEvent(h => Editor.ProfileChanged += h, h => Editor.ProfileChanged -= h)
            .Merge(selectionChanged)
            .StartWith(Unit.Default)
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Select(_ => CreateChart(Editor))
            .Subscribe(x => CurrentChart = x);

        SortedProfiles.MoveCurrentToFirst();
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

    public IProfileDirectory ProfileDirectory { get; }

    public ICollectionView SortedProfiles { get; }

    public INamedProfile CurrentProfile => (INamedProfile)SortedProfiles.CurrentItem;

    public ProfileEditor Editor => CurrentProfile.Editor;

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

    public bool ViewPersonalityAndDesign
    {
        get;
        set
        {
            if (Set(ref field, value))
            {
                NotifyOfPropertyChange(nameof(FilteredChart));
            }
        }
    } = true;

    public bool ViewDesignOnly
    {
        get;
        set
        {
            if (Set(ref field, value))
            {
                NotifyOfPropertyChange(nameof(FilteredChart));
            }
        }
    }

    public bool ViewPersonalityOnly
    {
        get;
        set
        {
            if (Set(ref field, value))
            {
                NotifyOfPropertyChange(nameof(FilteredChart));
            }
        }
    }

    public bool ShowStarPearl
    {
        get;
        set
        {
            if (Set(ref field, value))
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
        get;
        set
        {
            if (Set(ref field, value))
            {
                NotifyOfPropertyChange(nameof(SelectedHexagram));
                NotifyOfPropertyChange(nameof(SelectedGate));
                NotifyOfPropertyChange(nameof(NavigateToProgrammingPartnerCommand));
            }
        }
    }

    public ChartValue SelectedChartValue
    {
        get;
        set
        {
            if (Set(ref field, value))
            {
                SelectedGeneKey = SelectedChartValue != null ? GeneKeys[SelectedChartValue.Gate.Number] : null;
            }
        }
    }

    [UsedImplicitly]
    public async Task AddNew()
    {
        var nameEditor = CreateNewProfileNameEditor();
        await nameEditor.ShowModalAsync();

        SortedProfiles.MoveCurrentTo(ProfileDirectory.AddNew(nameEditor.NewName));
    }

    public bool CanRename => CurrentProfile is not null;

    [UsedImplicitly]
    public async Task Rename()
    {
        if (CanRename)
        {
            var nameEditor = CreateRenameEditor();
            await nameEditor.ShowModalAsync();

            CurrentProfile.Name = nameEditor.NewName;
        }
    }

    public bool CanDelete => CurrentProfile is not null && ProfileDirectory.Count > 1;

    [UsedImplicitly]
    public void Delete()
    {
        if (CanDelete && Prompt.Confirm($"Are you sure you want to delete the profile '{CurrentProfile}'?") == UserResponse.Accept)
        {
            CurrentProfile.Delete();
        }
    }

    private PersonalChart CurrentChart
    {
        get;
        set
        {
            if (Set(ref field, value))
            {
                NotifyOfPropertyChange(nameof(FilteredChart));
                NotifyOfPropertyChange(nameof(Points));
                UpdateSelectedFigures();
                SelectedChartValue = CurrentChart?.Values.FirstOrDefault();
            }
        }
    }

    private PersonalChart CreateChart(ProfileEditor editor)
    {
        return editor.IsComplete
            ? PersonalChart.Create(_gateDictionary, editor.GetValue)
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
