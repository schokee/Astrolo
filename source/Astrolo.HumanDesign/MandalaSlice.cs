using Astrolo.Astrology;
using Astrolo.Geometry;

namespace Astrolo.HumanDesign;

public sealed class MandalaSlice(ZodiacSign sign, Angle startAngle, Angle sweepAngle)
{
    public ZodiacSign Sign { get; } = sign;

    public ZodiacSignInfo SignInfo => Sign;

    public Angle StartAngle { get; } = startAngle;

    public Angle SweepAngle { get; } = sweepAngle;
}
