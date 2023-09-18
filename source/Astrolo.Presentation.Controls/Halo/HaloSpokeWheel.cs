using Astrolo.Geometry;

namespace Astrolo.Presentation.Controls.Halo
{
    public class HaloSpokeWheel : HaloShape
    {
        #region Dependency Properties

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
            nameof(StartAngle), typeof(double), typeof(HaloSpokeWheel), new PropertyMetadata(0.0, Refresh));

        public double StartAngle
        {
            get => (double)GetValue(StartAngleProperty);
            set => SetValue(StartAngleProperty, value);
        }

        public static readonly DependencyProperty SpokesProperty = DependencyProperty.Register(
            nameof(Spokes), typeof(int), typeof(HaloSpokeWheel), new PropertyMetadata(default(int), Refresh));

        public int Spokes
        {
            get => (int)GetValue(SpokesProperty);
            set => SetValue(SpokesProperty, value);
        }

        #endregion

        private readonly GeometryGroup _geometry = new();

        public HaloSpokeWheel()
        {
            DefiningGeometry = _geometry;
        }

        protected override System.Windows.Media.Geometry DefiningGeometry { get; }

        protected override void Arrange(Circle circle)
        {
            _geometry.Children.Clear();

            if (Spokes > 0)
            {
                var startAngle = Angle.FromDegrees(StartAngle);
                var sweepAngle = Seconds.ToAngle(Seconds.PerCircle / Spokes);

                var rays = Enumerable.Range(0, Spokes).Select(n => new LineGeometry
                {
                    StartPoint = circle.Center.ToPoint(),
                    EndPoint = circle.PointAt(startAngle + sweepAngle * n).ToPoint()
                });

                foreach (var line in rays)
                {
                    _geometry.Children.Add(line);
                }
            }
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
