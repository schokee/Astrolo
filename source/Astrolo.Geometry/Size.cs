using System;

namespace Astrolo.Geometry
{
    public readonly struct Size
    {
        public Size(double width, double height)
        {
            Width = Math.Abs(width);
            Height = Math.Abs(height);
        }

        public double Width { get; }

        public double Height { get; }
    }
}
