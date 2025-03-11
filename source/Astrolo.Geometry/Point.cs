namespace Astrolo.Geometry
{
    public readonly struct Point(double x, double y)
    {
        public double X { get; } = x;

        public double Y { get; } = y;

        public static Point Zero => new();

        public static Point operator -(Point p)
        {
            return new Point(-p.X, -p.Y);
        }

        public static Point operator +(Point p, Size o)
        {
            return new Point(p.X + o.Width, p.Y + o.Height);
        }

        public static Point operator -(Point p, Size o)
        {
            return new Point(p.X - o.Width, p.Y - o.Height);
        }

        public static implicit operator Size(Point p)
        {
            return new Size(p.X, p.Y);
        }
    }
}
