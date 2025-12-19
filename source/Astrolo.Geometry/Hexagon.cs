using System;

namespace Astrolo.Geometry
{
    public readonly struct Hexagon
    {
        public static double UnitHeight { get; } = Math.Sqrt(3);

        public Hexagon(double radius)
        {
            if (radius < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius));
            }

            Radius = radius;
        }

        public double Radius { get; }

        public double Apotherm => Radius * UnitHeight / 2;
    }
}
