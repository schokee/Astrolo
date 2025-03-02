using Astrolo.Geometry;
using Astrolo.YiJing;

namespace Astrolo.HumanDesign;

public sealed partial class GateDictionary
{
    private sealed class GateInfo : IGateInfo
    {
        private static Angle SweepAngle { get; } = -Seconds.ToAngle(MandalaGeometry.SecondsPerHexagram);

        private readonly IReadOnlyList<ILineInfo> _lines;
        private readonly HashSet<IChannel> _channels = [];

        public GateInfo(HexagramFigure hexagram, string name, Center center, Circuit circuit, IEnumerable<Serialization.LineRecord> lines)
        {
            _lines = lines.Select(x => (ILineInfo)new LineInfo(this, x)).ToList();

            Hexagram = hexagram;
            Name = name;
            Center = center;
            Circuit = circuit;
            MandalaSlice = new(_lines[0].MandalaSlice.Sign, hexagram.StartAngle(), SweepAngle);
        }

        public ILineInfo this[int line]
        {
            get => line is < 1 or > HexagramInfo.TotalLines
                ? throw new ArgumentOutOfRangeException(nameof(line))
                : _lines[line - 1];
        }

        public string Name { get; }
        public int Number => Hexagram.Number;
        public MandalaSlice MandalaSlice { get; }
        public HexagramFigure Hexagram { get; }
        public IReadOnlyCollection<ILineInfo> Lines => _lines;
        public Center Center { get; }
        public Circuit Circuit { get; }
        public IReadOnlyCollection<IChannel> Channels => _channels;
        public IEnumerable<int> Harmonics => this.SelectHarmonics(x => x.Number).OrderBy(x => x);

        public void Add(Channel channel)
        {
            _channels.Add(channel);
            channel.Gates.Add(this);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return format switch
            {
                "N" => Name,
                _ => ToString()
            };
        }

        public override string ToString()
        {
            return "The Gate of " + Name;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is IGateInfo gate && gate.Number == Number;
        }
    }
}
