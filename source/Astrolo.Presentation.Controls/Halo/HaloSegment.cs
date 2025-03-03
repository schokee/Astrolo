using Astrolo.Geometry;

namespace Astrolo.Presentation.Controls.Halo
{
    public class HaloSegment : HaloShape
    {
        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            nameof(Thickness), typeof(double), typeof(HaloSegment), new PropertyMetadata(default(double)));

        public double Thickness
        {
            get => (double)GetValue(ThicknessProperty);
            set => SetValue(ThicknessProperty, value);
        }

        public static readonly DependencyProperty InsetProperty = DependencyProperty.Register(
            nameof(Inset), typeof(double), typeof(HaloSegment), new PropertyMetadata(default(double)));

        public double Inset
        {
            get => (double)GetValue(InsetProperty);
            set => SetValue(InsetProperty, value);
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            nameof(Angle), typeof(double), typeof(HaloSlice), new PropertyMetadata(0.0, Refresh));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty SpreadProperty = DependencyProperty.Register(
            nameof(Spread), typeof(double), typeof(HaloSlice), new PropertyMetadata(360.0, Refresh));

        public double Spread
        {
            get => (double)GetValue(SpreadProperty);
            set => SetValue(SpreadProperty, value);
        }

        private readonly LineSegment _startCap = new();
        private readonly ArcSegment _outerArcSegment = new();
        private readonly LineSegment _endCap = new();
        private readonly ArcSegment _innerArcSegment = new();
        private readonly PathFigure _figure = new();

        public HaloSegment()
        {
            _outerArcSegment.SweepDirection = SweepDirection.Clockwise;
            _innerArcSegment.SweepDirection = SweepDirection.Counterclockwise;

            _figure.Segments = new PathSegmentCollection
            {
                _startCap,
                _outerArcSegment,
                _endCap,
                _innerArcSegment
            };

            DefiningGeometry = new PathGeometry
            {
                Figures = new PathFigureCollection { _figure }
            };
        }

        protected override System.Windows.Media.Geometry DefiningGeometry { get; }

        protected override void Arrange(Circle circle)
        {
            var innerRadius = System.Math.Max(0, circle.Radius - Thickness + StrokeThickness);
            var innerCircle = circle.ReduceBy(circle.Radius - innerRadius);

            var innerOffset = Inset > 0 ? Geometry.Angle.Asin(Inset / innerCircle.Radius) : Geometry.Angle.Zero;
            var outerOffset = Inset > 0 ? Geometry.Angle.Asin(Inset / circle.Radius) : Geometry.Angle.Zero;

            _figure.StartPoint = innerCircle.PointAt(Geometry.Angle.FromDegrees(Angle) + innerOffset).ToPoint();

            _startCap.Point = circle.PointAt(Geometry.Angle.FromDegrees(Angle) + outerOffset).ToPoint();

            _outerArcSegment.Point = circle.PointAt(Geometry.Angle.FromDegrees(Angle + Spread) - outerOffset).ToPoint();
            _outerArcSegment.Size = circle.QuadrantSize.ToSize();
            _outerArcSegment.IsLargeArc = Spread > 180;

            _endCap.Point = innerCircle.PointAt(Geometry.Angle.FromDegrees(Angle + Spread) - innerOffset).ToPoint();

            _innerArcSegment.Point = _figure.StartPoint;
            _innerArcSegment.Size = innerCircle.QuadrantSize.ToSize();
            _innerArcSegment.IsLargeArc = Spread > 180;
        }

        private static void Refresh(object o, DependencyPropertyChangedEventArgs e)
        {
            if (o is HaloShape shape)
            {
                shape.InvalidateMeasure();
                shape.UpdateLayout();
            }
        }
    }
}
