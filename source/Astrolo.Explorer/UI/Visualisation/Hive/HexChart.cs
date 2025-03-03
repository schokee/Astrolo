using System.Collections;
using System.Globalization;
using System.IO;
using Astrolo.Geometry;
using Astrolo.Geometry.PointyTop;
using Astrolo.YiJing;
using MoreLinq;
using Point = Astrolo.Geometry.PointyTop.Point;

namespace Astrolo.Explorer.UI.Visualisation.Hive;

public static class HexChart
{
    public class Statistics
    {
        private IReadOnlyCollection<HexagramFigure> Hexagrams { get; }

        public Statistics(IEnumerable<HexagramFigure> hexagrams)
        {
            Hexagrams = hexagrams.ToList();
        }

        public IEnumerable<uint> AllValues => Hexagrams.Select(x => x.Ordinal).OrderBy(x => x);

        public IEnumerable<uint> DistinctValues => AllValues.Distinct();

        public int Count => Hexagrams.Count;

        public int DistinctCount => DistinctValues.Count();

        public IEnumerable<uint> ReplicatedValues => AllValues.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key);

        public string ToString(bool asOctal)
        {
            return FormatValues(AllValues, asOctal);
        }

        private static string FormatValues(IEnumerable<uint> values, bool asOctal = false)
        {
            return string.Join(", ", values.Select(x => asOctal ? Convert.ToString(x, 8).PadLeft(2, '0') : x.ToString("00")));
        }
    }

    public static void SaveToFile(this ISequence sequence, string path)
    {
        using var stream = new StreamWriter(path);
        using var writer = new CsvHelper.CsvWriter(stream, CultureInfo.CurrentCulture);
        writer.WriteRecords((IEnumerable)sequence.GenerateSpherePoints());
    }

    public static IEnumerable<YiSpherePoint> GenerateSpherePoints(this ISequence sequence)
    {
        var points = GenerateLowerChart(sequence, 1)
            .Concat(GenerateUpperChart(sequence, 1))
            .DistinctBy(x => x.Value)
            .Select(cell =>
            {
                var point = cell.Location.ToCartesianPoint();

                return new YiSpherePoint
                {
                    Ordinal = cell.Value,
                    Sphere = cell.Ring == 4 ? 1 : cell.Ring < 4 ? 0 : 2,
                    Level = cell.Hexagram.Count(x => x.IsYin),
                    Angle = cell.Location == Point.Origin ? 0 : (int) Math.Round(Angle.Atan2(point.X, point.Y).Degrees)
                };
            })
            .OrderBy(x => x.Level)
            .ThenBy(x => x.Angle);

        return points;
    }

    public static IEnumerable<HexChartCell> GenerateUpperChart(this ISequence sequence, double radius)
    {
        return new UpperChartGenerator(radius, sequence.Hexagrams).GenerateCells();
    }

    public static IEnumerable<HexChartCell> GenerateLowerChart(this ISequence sequence, double radius)
    {
        return new LowerChartGenerator(radius, sequence.Hexagrams).GenerateCells();
    }

    private abstract class ChartGenerator
    {
        private double Radius { get; }
        private IReadOnlyList<HexagramFigure> Hexagrams { get; }

        protected ChartGenerator(uint startValue, double radius, IReadOnlyList<HexagramFigure> hexagrams)
        {
            StartValue = startValue;
            Radius = radius;
            Hexagrams = hexagrams;
        }

        public IEnumerable<HexChartCell> GenerateCells()
        {
            return GenerateRings(StartValue).SelectMany(x => x).ToList();
        }

        protected uint StartValue { get; }

        protected abstract uint Combine(uint v1, uint v2);

        protected abstract IEnumerable<HexChartCell> Ring1 { get; }
        protected abstract IEnumerable<HexChartCell> Ring6 { get; }

        protected virtual IEnumerable<HexChartCell> GenerateRing2(IReadOnlyCollection<HexChartCell> ring1)
        {
            uint GetValue(Point p) => Combine(p.EnumeratePerimeter(1).Join(ring1, x => x, x => x.Location, (_, x) => x.Value));

            // ReSharper disable once InvokeAsExtensionMethod
            return Enumerable.Concat(
                Point.Origin.DiagonalNeighbours.Select(p => CreateCell(2, p, GetValue(p))),
                Point.Origin.Neighbours.Select(p => CreateCell(2, p + p - Point.Origin, GetValue(p))));
        }

        protected virtual IEnumerable<HexChartCell> GenerateRing3(IReadOnlyCollection<HexChartCell> ring2)
        {
            return Point.AllFaces.Select(face =>
            {
                var source = Point.Origin.NeighborAt(face).Scale(2);

                return CreateCell(3, source.NeighborAt(face),
                    Combine(source.EnumeratePerimeter(1).Join(ring2, x => x, x => x.Location, (_, x) => x.Value)));
            });
        }

        protected virtual IEnumerable<HexChartCell> GenerateRing4(IReadOnlyCollection<HexChartCell> ring2, IReadOnlyCollection<HexChartCell> ring3)
        {
            return Point.Origin
                .EnumeratePerimeter(3)
                .Except(ring3.Select(x => x.Location))
                .Select((x, n) => (Key: n % 2, Value: x))
                .GroupBy(x => x.Key, x => x.Value)
                .SelectMany((targetPoints, n) =>
                {
                    var sourceA = ring2.Join(Point.Origin.DiagonalNeighbours, x => x.Location, x => x, (c, _) => c);
                    var sourceB = ring2.Join(Point.Origin.Neighbours.Select(x => x.Scale(2)), x => x.Location, x => x, (c, _) => c);
                    var values = sourceA.Zip(sourceB.Repeat().Skip(n == 0 ? 5 : 2), Combine);

                    return targetPoints.Zip(values, (p, v) => CreateCell(4, p, v));
                });
        }

        protected HexChartCell CreateCell(int ring, Point location, uint value)
        {
            return new(ring, Hexagrams[(int)value], location, Radius);
        }

        private uint Combine(IEnumerable<uint> values)
        {
            return values.Aggregate(StartValue, Combine);
        }

        private uint Combine(HexChartCell a, HexChartCell b)
        {
            return Combine(a.Value, b.Value);
        }

        private IEnumerable<IEnumerable<HexChartCell>> GenerateRings(uint startValue)
        {
            yield return MoreEnumerable.Return(CreateCell(0, Point.Origin, startValue));

            var ring1 = Ring1.ToList();
            var ring2 = GenerateRing2(ring1).ToList();
            var ring3 = GenerateRing3(ring2).ToList();
            var ring4 = GenerateRing4(ring2, ring3);
            var ring5 = ring1.Take(3).Zip(ring1.Repeat().Skip(3), (a, b) => CreateCell(5, a.Location.Scale(4), Combine(a, b)));

            yield return ring1;
            yield return ring2;
            yield return ring3;
            yield return ring4;
            yield return ring5;
            yield return Ring6;
        }
    }

    private class LowerChartGenerator : ChartGenerator
    {
        public LowerChartGenerator(double radius, IReadOnlyList<HexagramFigure> hexagrams) : base(0, radius, hexagrams)
        {
        }

        protected override uint Combine(uint v1, uint v2)
        {
            return v1 | v2;
        }

        protected override IEnumerable<HexChartCell> Ring1 => Point.Origin
            .EnumeratePerimeter(1)
            .Select((point, n) => CreateCell(1, point, Combine(StartValue, 1u << n)));

        protected override IEnumerable<HexChartCell> Ring6 => MoreEnumerable.Return(CreateCell(6, Point.Origin.NeighborAt(Face.TopLeft).Scale(4), 42));
    }

    private class UpperChartGenerator : ChartGenerator
    {
        public UpperChartGenerator(double radius, IReadOnlyList<HexagramFigure> hexagrams) : base((1 << 6) - 1, radius, hexagrams)
        {
        }

        protected override uint Combine(uint v1, uint v2)
        {
            return v1 & v2;
        }

        protected override IEnumerable<HexChartCell> Ring1 => Point.Origin
            .EnumeratePerimeter(1)
            .Select((point, n) => CreateCell(1, Point.Origin - point, Combine(StartValue, ~(1u << n))));

        protected override IEnumerable<HexChartCell> Ring6 => MoreEnumerable.Return(CreateCell(6, Point.Origin.NeighborAt(Face.BottomRight).Scale(4), 21));
    }
}
