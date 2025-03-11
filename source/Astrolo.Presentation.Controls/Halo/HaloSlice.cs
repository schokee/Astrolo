using Astrolo.Geometry;

namespace Astrolo.Presentation.Controls.Halo
{
    public class HaloSlice : HaloShape
    {
        #region Dependency Properties

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            nameof(Angle), typeof(double), typeof(HaloSlice), new PropertyMetadata(0.0, Refresh));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            nameof(Offset), typeof(double), typeof(HaloSlice), new PropertyMetadata(0.0, Refresh));

        public double Offset
        {
            get => (double)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public static readonly DependencyProperty SpreadProperty = DependencyProperty.Register(
            nameof(Spread), typeof(double), typeof(HaloSlice), new PropertyMetadata(360.0, Refresh));

        public double Spread
        {
            get => (double)GetValue(SpreadProperty);
            set => SetValue(SpreadProperty, value);
        }

        #endregion

        private readonly LineSegment _sliceStart = new();
        private readonly LineSegment _sliceEnd = new();
        private readonly ArcSegment _arcSegment = new();
        private readonly PathFigure _arcMarker = new();

        public HaloSlice()
        {
            _arcSegment.SweepDirection = SweepDirection.Clockwise;

            _arcMarker.Segments =
            [
                _sliceStart,
                _arcSegment,
                _sliceEnd
            ];

            DefiningGeometry = new PathGeometry
            {
                Figures = [_arcMarker]
            };
        }

        protected override System.Windows.Media.Geometry DefiningGeometry { get; }

        protected override void Arrange(Circle circle)
        {
            _arcMarker.StartPoint = circle.Center.ToPoint();

            _sliceStart.Point = circle.PointAt(Geometry.Angle.FromDegrees(Angle + Offset)).ToPoint();
            _sliceEnd.Point = circle.Center.ToPoint();

            _arcSegment.Point = circle.PointAt(Geometry.Angle.FromDegrees(Angle + Offset + Spread)).ToPoint();
            _arcSegment.Size = circle.QuadrantSize.ToSize();
            _arcSegment.IsLargeArc = Spread > 180;
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
