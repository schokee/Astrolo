using Astrolo.Geometry;

namespace Astrolo.Presentation.Controls.Halo
{
    public class HaloArc : HaloShape
    {
        #region Dependency Properties

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            nameof(Angle), typeof(double), typeof(HaloArc),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty SpreadProperty = DependencyProperty.Register(
            nameof(Spread), typeof(double), typeof(HaloArc),
            new FrameworkPropertyMetadata(90.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public double Spread
        {
            get => (double)GetValue(SpreadProperty);
            set => SetValue(SpreadProperty, value);
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            nameof(Offset), typeof(double), typeof(HaloArc),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public double Offset
        {
            get => (double)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public static readonly DependencyProperty TensionProperty = DependencyProperty.Register(
            nameof(Tension), typeof(double), typeof(HaloArc),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public double Tension
        {
            get => (double)GetValue(TensionProperty);
            set => SetValue(TensionProperty, value);
        }

        #endregion

        private readonly ArcSegment _segment = new() { SweepDirection = SweepDirection.Clockwise };
        private readonly PathFigure _figure = new();

        public HaloArc()
        {
            _figure.Segments = new PathSegmentCollection { _segment };

            DefiningGeometry = new PathGeometry
            {
                Figures = new PathFigureCollection { _figure }
            };

            BindingOperations.SetBinding(this, ConcentricPanel.ThicknessProperty, new Binding
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(nameof(StrokeThickness))
            });
        }

        protected override System.Windows.Media.Geometry DefiningGeometry { get; }

        protected override void Arrange(Circle circle)
        {
            var tension = Tension % 1;
            var angle = Angle + Offset;

            var startAngle = angle - tension * Spread;
            var endAngle = angle + (1 - tension) * Spread;

            _figure.StartPoint = circle.PointAt(Geometry.Angle.FromDegrees(startAngle)).ToPoint();

            _segment.Point = circle.PointAt(Geometry.Angle.FromDegrees(endAngle)).ToPoint();
            _segment.Size = circle.QuadrantSize.ToSize();
            _segment.IsLargeArc = Spread > 180;
        }
    }
}
