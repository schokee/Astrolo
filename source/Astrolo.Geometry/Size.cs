using System;

namespace Astrolo.Geometry
{
    public readonly struct Size(double width, double height)
    {
        public double Width { get; } = Math.Abs(width);

        public double Height { get; } = Math.Abs(height);
    }
}
