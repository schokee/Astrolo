using System;

namespace Astrolo.Geometry
{
    public readonly struct Circle : IEquatable<Circle>
    {
        public Circle(Point center, double radius)
        {
            if (radius < 0) throw new ArgumentOutOfRangeException(nameof(Radius));

            Center = center;
            Radius = radius;
        }

        public double Diameter => Radius * 2;

        public double Radius { get; }

        public Point Center { get; }

        public Point PointAt(Angle angle)
        {
            return new Point(
                Center.X + Radius * angle.Sin(),
                Center.Y - Radius * angle.Cos());
        }

        public Rectangle Bounds => new(Center.X - Radius, Center.Y - Radius, Diameter, Diameter);

        public Size Size => new(Diameter, Diameter);

        public Size QuadrantSize => new(Radius, Radius);

        public Circle GrowBy(double offset)
        {
            return new Circle(Center, Radius + offset);
        }

        public Circle ReduceBy(double offset)
        {
            return new Circle(Center, Math.Max(0, Radius - offset));
        }

        public static Circle CenterWithin(Size bounds)
        {
            return new Circle(new Point(bounds.Width / 2, bounds.Height / 2), bounds.SmallestSize() / 2);
        }

        public static Circle CenterWithin(Rectangle bounds)
        {
            return new Circle(bounds.Center, bounds.SmallestSize() / 2);
        }

        public bool Equals(Circle other)
        {
            return Radius.Equals(other.Radius) && Center.Equals(other.Center);
        }

        public override bool Equals(object? obj)
        {
            return obj is Circle other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Radius.GetHashCode() * 397) ^ Center.GetHashCode();
            }
        }

        public static bool operator ==(Circle left, Circle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Circle left, Circle right)
        {
            return !left.Equals(right);
        }
    }
}
