using Astrolo.Geometry;
using Astrolo.YiJing;

namespace Astrolo.HumanDesign;

public static class MandalaGeometry
{
    private static IReadOnlyList<Angle> Lookup { get; } = CreateLookup();

    public const int SecondsPerHexagram = Seconds.PerCircle / HexagramInfo.TotalHexagrams;

    public const int SecondsPerLine = SecondsPerHexagram / HexagramInfo.TotalLines;

    public static IEnumerable<(HexagramFigure Item, Angle StartAngle)> InTransitOrder(this IEnumerable<HexagramFigure> source)
    {
        return source.InTransitOrder(x => x);
    }

    public static Angle StartAngle(this HexagramFigure hexagram)
    {
        return Lookup[(int)hexagram.Ordinal];
    }

    public static Angle LineStartAngle(this HexagramFigure hexagram, int line)
    {
        if (line is < 1 or > HexagramInfo.TotalLines) throw new ArgumentOutOfRangeException(nameof(line));
        return hexagram.StartAngle() - Seconds.ToAngle((line - 1) * SecondsPerLine);
    }

    public static IEnumerable<(T Item, Angle StartAngle)> InTransitOrder<T>(this IEnumerable<T> source, Func<T, HexagramFigure> select)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return source
            .Select(x => (Item: x, StartAngle: select(x).StartAngle()))
            .OrderByDescending(x => x.StartAngle.Degrees);
    }

    private static IReadOnlyList<Angle> CreateLookup()
    {
        var trigrams = new[]
            {
                Trigram.Heaven,     // 111
                Trigram.Lake,       // 011
                Trigram.Fire,       // 101
                Trigram.Thunder,    // 001
                Trigram.Wind,       // 110
                Trigram.Water,      // 010
                Trigram.Mountain,   // 100
                Trigram.Earth       // 000
            }
            .Select(x => x.ToBitArray())
            .ToArray();

        var halfCircle = trigrams
            .Take(4)
            .SelectMany(lowerTrigram => trigrams.Select(upperTrigram => upperTrigram.Append(lowerTrigram)))
            .ToList();

        // Assumes Capricorn starts at 0°, (Gate 1 starts at 13°15'00" in Scorpio)
        var start = Seconds.FromDegrees(46, 45);

        return halfCircle
            .Concat(halfCircle.Select(x => x.Complement))
            .Select((x, n) => (x.Value, StartAngle: Seconds.ToAngle(start - n * SecondsPerHexagram)))
            .OrderBy(x => x.Value)
            .Select(x => x.StartAngle)
            .ToList();
    }
}
