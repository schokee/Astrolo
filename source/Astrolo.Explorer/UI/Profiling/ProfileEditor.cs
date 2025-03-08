using Astrolo.Astrology;
using Astrolo.GeneKeys;
using Astrolo.HumanDesign.Charting;
using Astrolo.YiJing;
using Caliburn.Micro;
using MoreLinq;

namespace Astrolo.Explorer.UI.Profiling;

public sealed class ProfileEditor : PropertyChangedBase
{
    private Func<int, IGeneKey> GetGeneKey { get; }

    public event System.Action ProfileChanged;

    public ProfileEditor(Func<int, IGeneKey> getGeneKey)
    {
        GetGeneKey = getGeneKey;
        DesignPoints = CreatePointCollection(true);
        PersonalityPoints = CreatePointCollection(false);

        IReadOnlyCollection<ProfilePoint> CreatePointCollection(bool isDesign) => OrderedMarkers
            .Select(marker => new ProfilePoint(this, marker, isDesign))
            .Pipe(x => x.ValueChanged += OnPointChanged)
            .ToList();
    }

    public ProfilePoint this[Marker marker, bool design]
    {
        get
        {
            var source = design ? DesignPoints : PersonalityPoints;
            return source.First(x => x.Marker == marker);
        }
    }

    public IEnumerable<ProfilePoint> Points => DesignPoints.Concat(PersonalityPoints);

    public bool IsComplete => Points.All(x => x.Value is not null) && !IsIncarnationCrossInvalid;

    private bool _isIncarnationCrossInvalid;
    public bool IsIncarnationCrossInvalid
    {
        get => _isIncarnationCrossInvalid;
        private set
        {
            if (Set(ref _isIncarnationCrossInvalid, value))
            {
                NotifyOfPropertyChange(nameof(IsComplete));
            }
        }
    }

    public IEnumerable<ProfilePoint> IncarnationCrossPoints =>
        new[] {false, true}.Cartesian(OrderedMarkers.Take(2), (isDesign, marker) => this[marker, isDesign]);

    public IReadOnlyCollection<ProfilePoint> DesignPoints { get; }

    public IReadOnlyCollection<ProfilePoint> PersonalityPoints { get; }

    public LineOfHexagram GetValue(Marker marker, bool isDesign)
    {
        return this[marker, isDesign].Value!.Value;
    }

    public void Clear()
    {
        InitializeFrom(Points.Select(_ => string.Empty));
    }

    public void InitializeFrom(string designAndPersonality)
    {
        InitializeFrom(designAndPersonality.Split(';'));
    }

    public void InitializeFrom(IEnumerable<string> values)
    {
        var pairs = Points.Zip(values, (t, value) => (Target: t, Value: value));

        foreach ((ProfilePoint target, string value) in pairs)
        {
            target.ValueAsText = value;
        }
    }

    public string Validate(ProfilePoint point)
    {
        var value = point.Value;

        if (value == null)
        {
            return "Invalid value";
        }

        if (point.Marker == Marker.Sun)
        {
            var earthValue = GetGeneKey(value.Value.Hexagram)
                .ProgrammingPartner
                .ToLineOfHexagram(value.Value.Line);

            this[Marker.Earth, point.IsDesign].Value = earthValue;
        }

        return null;
    }

    private void OnPointChanged(ProfilePoint point)
    {
        if (point.Marker == Marker.Earth)
        {
            var points = IncarnationCrossPoints.Select(x => x.Value.GetValueOrDefault()).Where(x => x.Hexagram > 0).ToArray();
            IsIncarnationCrossInvalid = !PersonalChart.IsValidIncarnationCross(points);
        }

        NotifyOfPropertyChange(nameof(IsComplete));
        ProfileChanged?.Invoke();
    }

    private static IReadOnlyCollection<Marker> OrderedMarkers { get; } =
    [
        Marker.Sun,
        Marker.Earth,
        Marker.NorthNode,
        Marker.SouthNode,
        Marker.Moon,
        Marker.Mercury,
        Marker.Venus,
        Marker.Mars,
        Marker.Jupiter,
        Marker.Saturn,
        Marker.Uranus,
        Marker.Neptune,
        Marker.Pluto
    ];
}
