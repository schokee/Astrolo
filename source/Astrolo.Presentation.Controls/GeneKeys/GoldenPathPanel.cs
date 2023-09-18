using Astrolo.GeneKeys;
using Size = System.Windows.Size;

namespace Astrolo.Presentation.Controls.GeneKeys
{
    public sealed class GoldenPathPanel : Panel
    {
        #region SphereSizeProperty

        public static readonly DependencyProperty SphereSizeProperty = DependencyProperty.Register(nameof(SphereSize), typeof(double),
            typeof(GoldenPathPanel), new FrameworkPropertyMetadata(16d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double SphereSize
        {
            get => (double)GetValue(SphereSizeProperty);
            set => SetValue(SphereSizeProperty, value);
        }

        #endregion

        #region PointProperty

        public static readonly DependencyProperty PointProperty = DependencyProperty.RegisterAttached(
            "Point", typeof(Sphere), typeof(GoldenPathPanel), new PropertyMetadata(default(Sphere)));

        public static void SetPoint(DependencyObject element, Sphere value)
        {
            element.SetValue(PointProperty, value);
        }

        public static Sphere GetPoint(DependencyObject element)
        {
            return (Sphere)element.GetValue(PointProperty);
        }

        #endregion

        protected override Size MeasureOverride(Size constraint)
        {
            var bounds = base.MeasureOverride(constraint);
            var size = new ChartMetrics(SphereSize, new Geometry.Rectangle(0, 0, bounds.Width, bounds.Height)).Bounds.Size;

            return new Size(size.Width, size.Height);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var chartMetrics = new ChartMetrics(SphereSize, new Geometry.Rectangle(0, 0, arrangeBounds.Width, arrangeBounds.Height));

            foreach (var element in Children.OfType<UIElement>())
            {
                var circle = chartMetrics.CircleOf(GetPoint(element));
                element.Arrange(circle.Bounds.ToRect());
            }

            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
