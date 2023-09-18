using Astrolo.YiJing;

namespace Astrolo.Presentation.Controls
{
    internal static class UIExtensions
    {
        public static double MaxExtent(this Size size)
        {
            return Math.Max(size.Width, size.Height);
        }

        public static Size ToSize(this Geometry.Size s)
        {
            return new Size(s.Width, s.Height);
        }

        public static Point ToPoint(this Geometry.Point p)
        {
            return new Point(p.X, p.Y);
        }

        public static Rect ToRect(this Geometry.Rectangle r)
        {
            return new Rect(r.Left, r.Top, r.Width, r.Height);
        }

        public static Rect ReduceBy(this Rect rect, double size)
        {
            var offset = new Vector(size, size);
            return new Rect(rect.TopLeft + offset, rect.BottomRight - offset);
        }

        public static Color GetColor(this TrigramFigure trigram)
        {
            return trigram.Info.Key.GetColor();
        }

        public static Color GetColor(this Trigram trigram)
        {
            return trigram switch
            {
                Trigram.Earth => Colors.DimGray,
                Trigram.Thunder => Colors.YellowGreen,
                Trigram.Water => Colors.RoyalBlue,
                Trigram.Lake => Colors.BlueViolet,
                Trigram.Mountain => Colors.DarkRed,
                Trigram.Fire => Colors.DarkOrange,
                Trigram.Wind => Colors.Gold,
                Trigram.Heaven => Colors.LightGray,
                _ => throw new ArgumentOutOfRangeException(nameof(trigram), trigram, null)
            };
        }

        public static void InvalidateAndUpdateLayout(this object o)
        {
            if (o is UIElement element)
            {
                element.InvalidateMeasure();
                element.UpdateLayout();
            }
        }

        public static void InvalidateLayout(this object o)
        {
            if (o is UIElement element)
            {
                element.InvalidateMeasure();
                element.InvalidateArrange();
            }
        }
    }
}
