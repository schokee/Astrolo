using Astrolo.GeneKeys;
using Astrolo.HumanDesign;
using Astrolo.HumanDesign.Charting;
using Astrolo.YiJing;
using Caliburn.Micro;

namespace Astrolo.Explorer.Model;

public sealed class GeneKeyPoint : PropertyChangedBase, IComparable<GeneKeyPoint>, IComparable, IFormattable
{
    private bool _isEnabled = true;
    private bool _isSelected;

    public GeneKeyPoint(GeneKeyProfile profile, Sphere sphere, ChartValue chartValue)
    {
        Profile = profile;
        ChartValue = chartValue;
        Sphere = sphere;
    }

    public ChartValue ChartValue { get; }
    public GeneKeyProfile Profile { get; }
    public Sphere Sphere { get; }
    public IGeneKey GeneKey => Profile.GeneKeys[Gate.Number];
    public int Line => LineInfo.Number;

    public bool IsCenterDefined => Profile.Chart.Centers[Gate.Center] == CenterState.Defined;
    public IGateInfo Gate => ChartValue.Gate;
    public ILineInfo LineInfo => ChartValue.Line;

    public string Source => $"{(ChartValue.IsDesign ? "Unconscious" : "Conscious")} {ChartValue.Marker}";

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (value == _isEnabled) return;
            _isEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (value == _isSelected) return;
            _isSelected = value;
            NotifyOfPropertyChange();
        }
    }

    public Uri Link => new("https://genekeys.com/gene-key-" + $"{GeneKey.Number}/");

    public IEnumerable<(MatchFlags Match, GeneKeyPoint Point)> CompareWith(GeneKeyPoint point)
    {
        if (GeneKey == point.GeneKey)
        {
            yield return (MatchFlags.GeneKey | (Line == point.Line ? MatchFlags.Line : MatchFlags.None), point);
            yield break;
        }

        if (GeneKey == point.GeneKey.ProgrammingPartner)
        {
            yield return (MatchFlags.ComplimentaryKey, point);
        }

        if (Line == point.Line)
        {
            yield return (MatchFlags.Line, point);
        }
    }

    public LineOfHexagram ToKeyWithLine()
    {
        return new(GeneKey.Number, Line);
    }

    public override string ToString()
    {
        return $"{GeneKey.Number}.{Line}";
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        switch (format)
        {
            case "K":
                return $"{GeneKey.Number:00}.{Line}";

            case "n":
                return $"{GeneKey.Number}";

            case "g":
            default:
                return ToString();
        }
    }

    public int CompareTo(object obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is GeneKeyPoint other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(GeneKeyPoint)}");
    }

    public int CompareTo(GeneKeyPoint other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        var diff = GeneKey.Number.CompareTo(other.GeneKey.Number);
        return diff == 0 ? Line.CompareTo(other.Line) : diff;
    }
}
