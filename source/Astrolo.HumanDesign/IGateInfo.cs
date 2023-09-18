using Astrolo.YiJing;

namespace Astrolo.HumanDesign
{
    public interface IGateInfo : IFormattable
    {
        int Number { get; }

        string Name { get; }

        MandalaSlice MandalaSlice { get; }

        HexagramFigure Hexagram { get; }

        IReadOnlyCollection<ILineInfo> Lines { get; }

        ILineInfo this[int line] { get; }

        Center Center { get; }

        Circuit Circuit { get; }

        IReadOnlyCollection<IChannel> Channels { get; }

        IEnumerable<int> Harmonics { get; }
    }
}
