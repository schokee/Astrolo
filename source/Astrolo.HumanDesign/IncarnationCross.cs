using Astrolo.Core;
using Astrolo.HumanDesign.Metadata;
using Astrolo.YiJing;
using MoreLinq;

namespace Astrolo.HumanDesign;

public sealed class IncarnationCross : IComparable, IComparable<IncarnationCross>, IFormattable
{
    private string FormattedGates { get; }

    private IncarnationCross(IncarnationCrossType type, string partialName, IReadOnlyList<int> gates, int quarter, string text)
    {
        Type = type;
        PartialName = partialName;
        Gates = gates;
        Quarter = quarter;
        Text = text;
        FormattedGates = $" {gates[0]}-{gates[1]}/{gates[2]}-{gates[3]}";
    }

    public string PartialName { get; }

    public IncarnationCrossType Type { get; }

    public int Quarter { get; }

    public IReadOnlyCollection<int> Gates { get; }

    public string Text { get; }

    public string TypeAsString => Type.GetDescriptionOrDefault();

    public string TypeAsMnemonic => Mnemonics[Type];

    public string UnqualifiedName => $"{TypeAsString} Cross of {PartialName}";

    public string FullName => (Quarter > 0 ? $"{UnqualifiedName} {Quarter}" : UnqualifiedName);// + FormattedGates;

    public bool Matches(LineOfHexagram[] points)
    {
        if (points.Length != 4) throw new ArgumentException(nameof(points));

        return IsApplicableToDesign(points[0].Line, points[3].Line) && Gates.SequenceEqual(points.Select(x => x.Hexagram));
    }

    public bool IsApplicableToDesign(int personalityLine, int destinyLine)
    {
        return Type switch
        {
            IncarnationCrossType.Juxtaposition => personalityLine == 4 && destinyLine == 1,
            IncarnationCrossType.LeftAngle => personalityLine > 4,
            IncarnationCrossType.RightAngle => personalityLine < 4 || personalityLine == 4 && destinyLine == 6,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public int CompareTo(IncarnationCross? other)
    {
        if (ReferenceEquals(this, other)) return 0;

        if (ReferenceEquals(null, other)) return 1;

        var diff = Type.CompareTo(other.Type);

        if (diff == 0)
            diff = string.Compare(PartialName, other.PartialName, StringComparison.CurrentCulture);

        if (diff == 0)
            diff = Quarter.CompareTo(other.Quarter);

        return diff;
    }

    public int CompareTo(object? obj)
    {
        return CompareTo(obj as IncarnationCross);
    }

    public override string ToString()
    {
        return FullName;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        switch (format)
        {
            case "l":
                return $"{TypeAsMnemonic} of {PartialName}";//{FormattedGates}";

            case "s":
            {
                var prefix = $"{TypeAsMnemonic} of {PartialName}";
                return Quarter > 0 ? prefix + $" ({Quarter})" : prefix;
            }

            default:
                return ToString();
        }
    }

    public static IEnumerable<string> EnumerateDescriptions()
    {
        using var stream = EmbeddedFile.Open("IncarnationCrossInfo.txt");
        using var reader = new StreamReader(stream, Encoding.UTF8, true);

        for (var line = reader.ReadLine(); line is not null; line = reader.ReadLine())
        {
            yield return line;
        }
    }

    public static IEnumerable<IncarnationCross> Load()
    {
        return EnumerateDescriptions()
            .Batch(2)
            .Select(pair =>
            {
                var match = ParsePattern.Match(pair.First());
                return !match.Success ? null : CreateInfo(match.Groups, string.Empty);
            })
            .WhereNotNull()
            .OrderBy(x => x);
    }

    public static IEnumerable<(T Source, IncarnationCross IncarnationCross)> Lookup<T>(IEnumerable<T> source, Func<T, IEnumerable<LineOfHexagram>> getGates)
    {
        return source
            //.Pipe(x => Debug.WriteLine(x))
            .Select(x => (Source: x, Gates: getGates(x).Take(4).ToArray()))
            .GroupJoin(Load(), x => x.Gates.Select(k => k.Hexagram).ToArray(), x => x.Gates,
                (x, l) => (x.Source, IncarnationCross: l.Single(incarnationCross => incarnationCross.IsApplicableToDesign(x.Gates[0].Line, x.Gates[3].Line))),
                ArrayEqualityComparer<int>.Default);
    }

    private static IReadOnlyDictionary<IncarnationCrossType, string> Mnemonics { get; } = new Dictionary<IncarnationCrossType, string>
    {
        {IncarnationCrossType.Juxtaposition, "JUX"},
        {IncarnationCrossType.LeftAngle, "LAX"},
        {IncarnationCrossType.RightAngle, "RAX"}
    };

    private static Regex ParsePattern { get; } = new(
        @"^The (?<Type>Juxtaposition|Left Angle|Right Angle) Cross of (?<Name>.*?)( (?<Quarter>\d))? \((?<Sp>\d+)/(?<Ep>\d+)...(?<Sd>\d+)/(?<Ed>\d+)\)");

    private static IncarnationCross CreateInfo(GroupCollection match, string text)
    {
        var gates = new[] { "Sp", "Ep", "Sd", "Ed" }.Select(x => int.Parse(match[x].Value)).ToArray();

        var typeText = match["Type"].Value;
        var type =
            typeText == "Juxtaposition" ? IncarnationCrossType.Juxtaposition :
            typeText == "Left Angle" ? IncarnationCrossType.LeftAngle : IncarnationCrossType.RightAngle;

        int.TryParse(match["Quarter"].Value, out var quarter);
        return new(type, match["Name"].Value, gates, quarter, text);
    }
}
