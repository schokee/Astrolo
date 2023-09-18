using Astrolo.GeneKeys;
using Size = System.Windows.Size;

namespace Astrolo.Presentation.Controls.GeneKeys
{
    public sealed class GoldenPath : Shape
    {
        #region SphereSizeProperty

        public static readonly DependencyProperty SphereSizeProperty = DependencyProperty.Register(nameof(SphereSize), typeof(double),
            typeof(GoldenPath), new FrameworkPropertyMetadata(16d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        public double SphereSize
        {
            get => (double)GetValue(SphereSizeProperty);
            set => SetValue(SphereSizeProperty, value);
        }

        #endregion

        #region SequenceProperty

        public static readonly DependencyProperty SequenceProperty = DependencyProperty.Register(nameof(Sequence), typeof(Sequence),
            typeof(GoldenPath), new UIPropertyMetadata(default(Sequence), (o, _) => ((GoldenPath)o).InvalidateVisual()));

        public Sequence Sequence
        {
            get => (Sequence)GetValue(SequenceProperty);
            set => SetValue(SequenceProperty, value);
        }

        #endregion

        protected override Size MeasureOverride(Size constraint)
        {
            var size = GetMetrics(base.MeasureOverride(constraint)).Bounds.Size.ToSize();
            return size;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Metrics = GetMetrics(arrangeBounds);
            return base.ArrangeOverride(arrangeBounds);
        }

        protected override System.Windows.Media.Geometry DefiningGeometry => GetGeometry();

        private ChartMetrics? Metrics { get; set; }

        private ChartMetrics GetMetrics(Size size)
        {
            return new ChartMetrics(SphereSize, new Geometry.Rectangle(0, 0, size.Width, size.Height));
        }

        private System.Windows.Media.Geometry GetGeometry()
        {
            if (Metrics is null)
            {
                return new PathGeometry();
            }

            var segments = Metrics.PointsInSequence(Sequence).Select(x => new LineSegment(x.ToPoint(), true)).ToList();
            return new PathGeometry(new[] { new PathFigure(segments[0].Point, segments.Skip(1), false) });
        }
    }
}
