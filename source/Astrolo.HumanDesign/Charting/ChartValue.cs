using Astrolo.Astrology;
using Astrolo.YiJing;

namespace Astrolo.HumanDesign.Charting
{
    public sealed class ChartValue
    {
        public ChartValue(Marker marker, bool isDesign, ILineInfo line)
        {
            Marker = marker;
            IsDesign = isDesign;
            Line = line;
        }

        public Marker Marker { get; }

        public bool IsDesign { get; }

        public IGateInfo Gate => Line.Gate;

        public ILineInfo Line { get; }

        public LineOfHexagram LineOfHexagram => Line.ToLineOfHexagram();
    }
}
