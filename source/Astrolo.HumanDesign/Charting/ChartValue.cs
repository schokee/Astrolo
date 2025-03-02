using Astrolo.Astrology;
using Astrolo.YiJing;

namespace Astrolo.HumanDesign.Charting;

public sealed class ChartValue(Marker marker, bool isDesign, ILineInfo line)
{
    public Marker Marker { get; } = marker;

    public bool IsDesign { get; } = isDesign;

    public IGateInfo Gate => Line.Gate;

    public ILineInfo Line { get; } = line;

    public LineOfHexagram LineOfHexagram => Line.ToLineOfHexagram();
}
