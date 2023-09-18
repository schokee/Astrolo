using Astrolo.Astrology;
using Astrolo.Geometry;
using Astrolo.HumanDesign.Metadata;
using Astrolo.YiJing;

namespace Astrolo.HumanDesign
{
    public sealed partial class GateDictionary
    {
        public static GateDictionary Create()
        {
            var hexagramLookup = new KingWenSequence()
                .Hexagrams
                .ToDictionary(x => x.Number);

            var lineLookup = EmbeddedFile
                .DeserializeCsv<Serialization.LineRecord>("Lines.csv")
                .OrderBy(x => x.Line)
                .ToLookup(x => x.Gate);

            var channels = EmbeddedFile
                .DeserializeCsv<Serialization.GateRecord>("Gates.csv")
                .GroupBy(x => x.Channel!)
                .Where(x => x.Count() == 2);

            var allGates = new Dictionary<int, GateInfo>();

            foreach (var pair in channels)
            {
                var channel = new Channel(pair.Key);

                foreach (var info in pair)
                {
                    if (!allGates.TryGetValue(info.Number, out var gate))
                    {
                        gate = new GateInfo(hexagramLookup[info.Number], info.Gate!, info.Center, info.Circuit, lineLookup[info.Number]);
                        allGates[info.Number] = gate;
                    }

                    gate.Add(channel);
                }
            }

            return new GateDictionary(allGates.Values);
        }

        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable MemberHidesStaticFromOuterClass
        private static class Serialization
        {
            public class GateRecord
            {
                public int Number { get; set; }

                public string? Gate { get; set; }

                public string? Channel { get; set; }

                public Center Center { get; set; }

                public Circuit Circuit { get; set; }
            }

            public class LineRecord
            {
                private static Angle SweepAngle { get; } = Angle.FromDegrees(0, 56, 25);

                private static Angle Origin { get; } = Angle.FromDegrees(30);

                public int Gate { get; set; }

                public int Line { get; set; }

                public string? Theme { get; set; }

                public ZodiacSign Sign { get; set; }

                public string? Offset { get; set; }

                public MandalaSlice ToLocation()
                {
                    var startAngle = Origin + ChartMetrics.StartAngle(Sign) - Angle.ParseCoordinate(Offset!);
                    return new MandalaSlice(Sign, startAngle, SweepAngle);
                }

                public override string ToString()
                {
                    return Line.ToString();
                }
            }
        }
    }
}
