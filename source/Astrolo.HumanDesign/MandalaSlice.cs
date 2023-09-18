using Astrolo.Astrology;
using Astrolo.Geometry;

namespace Astrolo.HumanDesign
{
    public sealed class MandalaSlice
    {
        public MandalaSlice(ZodiacSign sign, Angle startAngle, Angle sweepAngle)
        {
            Sign = sign;
            StartAngle = startAngle;
            SweepAngle = sweepAngle;
        }

        public ZodiacSign Sign { get; }

        public ZodiacSignInfo SignInfo => Sign;

        public Angle StartAngle { get; }

        public Angle SweepAngle { get; }
    }
}
