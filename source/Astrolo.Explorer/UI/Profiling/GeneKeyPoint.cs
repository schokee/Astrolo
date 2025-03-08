using Astrolo.Explorer.Components;
using Astrolo.GeneKeys;
using Astrolo.HumanDesign;
using Astrolo.HumanDesign.Charting;
using Astrolo.YiJing;

namespace Astrolo.Explorer.UI.Profiling;

public sealed class GeneKeyPoint(IGeneKey geneKey, Sphere sphere, ChartValue chartValue, string lineNote, bool isCenterDefined)
    : Selectable, IComparable<GeneKeyPoint>, IComparable, IFormattable
{
    private bool _isEnabled = true;

    public ChartValue ChartValue { get; } = chartValue;
    public Sphere Sphere { get; } = sphere;
    public IGeneKey GeneKey => geneKey;
    public int Line => LineInfo.Number;
    public string LineNote { get; } = lineNote;

    public bool IsCenterDefined { get; }  = isCenterDefined;
    public IGateInfo Gate => ChartValue.Gate;
    public ILineInfo LineInfo => ChartValue.Line;

    public string Source => $"{(ChartValue.IsDesign ? "Unconscious" : "Conscious")} {ChartValue.Marker}";

    public bool IsEnabled
    {
        get => _isEnabled;
        set => Set(ref _isEnabled, value);
    }

    public Uri Link => new("https://genekeys.com/gene-key-" + $"{GeneKey.Number}/");

    public LineOfHexagram ToKeyWithLine()
    {
        return new LineOfHexagram(GeneKey.Number, Line);
    }

    public override string ToString()
    {
        return $"{GeneKey.Number}.{Line}";
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return format switch
        {
            "K" => $"{GeneKey.Number:00}.{Line}",
            "n" => $"{GeneKey.Number}",
            "g" => ToString(),
            _ => ToString()
        };
    }

    public int CompareTo(object obj)
    {
        if (ReferenceEquals(null, obj))
            return 1;
        if (ReferenceEquals(this, obj))
            return 0;
        return obj is GeneKeyPoint other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(GeneKeyPoint)}");
    }

    public int CompareTo(GeneKeyPoint other)
    {
        if (ReferenceEquals(this, other))
            return 0;
        if (ReferenceEquals(null, other))
            return 1;

        var diff = GeneKey.Number.CompareTo(other.GeneKey.Number);
        return diff == 0 ? Line.CompareTo(other.Line) : diff;
    }
}
