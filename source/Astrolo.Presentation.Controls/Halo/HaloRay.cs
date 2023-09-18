using Astrolo.Geometry;

namespace Astrolo.Presentation.Controls.Halo
{
    public class HaloRay : HaloShape
    {
        #region Dependency Properties

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            nameof(Angle), typeof(double), typeof(HaloRay), new PropertyMetadata(0.0, Refresh));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            nameof(Offset), typeof(double), typeof(HaloRay), new PropertyMetadata(0.0, Refresh));

        public double Offset
        {
            get => (double)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        #endregion

        private readonly LineGeometry _ray = new();

        public HaloRay()
        {
            DefiningGeometry = _ray;
        }

        protected override System.Windows.Media.Geometry DefiningGeometry { get; }

        protected override void Arrange(Circle circle)
        {
            _ray.StartPoint = circle.Center.ToPoint();
            _ray.EndPoint = circle.PointAt(Geometry.Angle.FromDegrees(Angle + Offset)).ToPoint();
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
