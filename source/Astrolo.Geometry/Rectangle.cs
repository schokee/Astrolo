namespace Astrolo.Geometry
{
    public readonly struct Rectangle
    {
        public Rectangle(Point topLeft, Size size) : this(topLeft.X, topLeft.Y, size.Width, size.Height)
        {
        }

        public Rectangle(double left, double top, double width, double height)
        {
            if (width < 0)
            {
                Left = left + width;
                Right = left;
            }
            else
            {
                Left = left;
                Right = left + width;
            }

            if (height < 0)
            {
                Top = top + height;
                Bottom = top;
            }
            else
            {
                Top = top;
                Bottom = top + height;
            }
        }

        public double Left { get; }
        public double Right { get; }

        public double Top { get; }
        public double Bottom { get; }

        public double Width => Right - Left;
        public double Height => Bottom - Top;

        public Size Size => new(Width, Height);

        public Point TopLeft => new(Left, Top);
        public Point BottomRight => new(Right, Bottom);
        public Point Center => new((Left + Right) / 2, (Top + Bottom) / 2);
    }
}
