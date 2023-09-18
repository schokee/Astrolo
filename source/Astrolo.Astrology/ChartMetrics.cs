using Astrolo.Geometry;

namespace Astrolo.Astrology
{
    public class ChartMetrics
    {
        public const int DegreesPerSign = 360 / 12;

        public ChartMetrics(Rectangle bounds)
        {
            Bounds = bounds;
            OuterCircle = Circle.CenterWithin(bounds);
        }

        public Rectangle Bounds { get; }

        public Circle OuterCircle { get; }

        public Point StartPoint(ZodiacSign sign)
        {
            return OuterCircle.PointAt(StartAngle(sign));
        }

        public static Angle StartAngle(ZodiacSign sign)
        {
            return Angle.FromDegrees((11 - (int)sign) * DegreesPerSign);
        }
    }
}
