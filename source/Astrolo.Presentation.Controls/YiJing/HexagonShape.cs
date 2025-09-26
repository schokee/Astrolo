using Astrolo.Geometry;
using Size = System.Windows.Size;

namespace Astrolo.Presentation.Controls.YiJing
{
    public class HexagonShape : Shape
    {
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(nameof(Radius), typeof(double),
            typeof(HexagonShape), new UIPropertyMetadata(1d, OnValueChanged));

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HexagonShape)d).InvalidateVisual();
        }

        protected override System.Windows.Media.Geometry DefiningGeometry => GetGeometry();

        protected override Size MeasureOverride(Size constraint)
        {
            if (Radius <= 0)
                return new Size(0, 0);

            var hexagon = new Hexagon((float)Radius);
            return new Size(hexagon.Apotherm * 2 + StrokeThickness, hexagon.Radius * 2 + StrokeThickness);
        }

        private System.Windows.Media.Geometry GetGeometry()
        {
            if (Radius <= 0)
            {
                return new PathGeometry();
            }

            var center = new Vector(ActualWidth, ActualHeight) / 2;
            var points = new Hexagon((float)(Radius - StrokeThickness / 2)).EnumeratePointyTopVertices().Select(x => x.ToPoint() + center).ToArray();

            return new PathGeometry
            {
                Figures =
                {
                    new PathFigure(points[0], [new PolyLineSegment(points.Skip(1), true)], true)
                },
                FillRule = FillRule.EvenOdd
            };
        }
    }
}
