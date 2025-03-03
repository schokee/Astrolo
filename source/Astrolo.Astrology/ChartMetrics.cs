using Astrolo.Geometry;

namespace Astrolo.Astrology;

public class ChartMetrics(Rectangle bounds)
{
    public const int DegreesPerSign = 360 / 12;

    public Rectangle Bounds { get; } = bounds;

    public Circle OuterCircle { get; } = Circle.CenterWithin(bounds);

    public Point StartPoint(ZodiacSign sign)
    {
        return OuterCircle.PointAt(StartAngle(sign));
    }

    public static Angle StartAngle(ZodiacSign sign)
    {
        return Angle.FromDegrees((11 - (int)sign) * DegreesPerSign);
    }
}
