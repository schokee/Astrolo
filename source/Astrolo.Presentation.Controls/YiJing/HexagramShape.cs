using Astrolo.YiJing;

namespace Astrolo.Presentation.Controls.YiJing
{
    public class HexagramShape : Shape
    {
        #region Number Property

        public static readonly DependencyProperty HexagramProperty = DependencyProperty.Register(nameof(Hexagram), typeof(HexagramFigure),
            typeof(HexagramShape), new UIPropertyMetadata(default(HexagramFigure), OnValueChanged));

        public HexagramFigure Hexagram
        {
            get => (HexagramFigure)GetValue(HexagramProperty);
            set => SetValue(HexagramProperty, value);
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HexagramShape)d).InvalidateVisual();
        }

        #endregion

        #region SpacerProperty

        public static readonly DependencyProperty SpacerProperty = DependencyProperty.Register(nameof(Spacer), typeof(float),
            typeof(HexagramShape), new UIPropertyMetadata(0.6f, OnSpacerChanged, CoerceSpacer));

        public float Spacer
        {
            get => (float)GetValue(SpacerProperty);
            set => SetValue(SpacerProperty, value);
        }

        private static void OnSpacerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HexagramShape)d).InvalidateVisual();
        }

        private static object CoerceSpacer(DependencyObject d, object baseValue)
        {
            return Math.Min(Math.Max(0, Convert.ToSingle(baseValue)), 1);
        }

        #endregion

        protected override System.Windows.Media.Geometry DefiningGeometry => GetGeometry();

        protected override Size MeasureOverride(Size constraint)
        {
            return constraint;
        }

        // Rendered bottom-up
        private System.Windows.Media.Geometry GetGeometry()
        {
            var result = new GeometryGroup();

            Render(Hexagram, new Size(ActualWidth, ActualHeight), Spacer, StrokeThickness, (_, bounds) => result.Children.Add(new RectangleGeometry(bounds)));
            return result;
        }

        public static void Render(HexagramFigure? hexagram, Size bounds, double spacer, double strokeThickness, Action<bool, Rect> render)
        {
            var lineHeight = bounds.Height / (6 + spacer * 5);

            var lineSize = new Size(bounds.Width - strokeThickness, lineHeight - strokeThickness);
            var yinSegmentSize = new Size(Math.Max(0, (lineSize.Width - lineSize.Height) / 2.2), lineSize.Height);

            var i = 0;
            var y = bounds.Height - lineSize.Height;

            // Rendered bottom-up
            foreach (var line in hexagram ?? Enumerable.Empty<Astrolo.YiJing.Line>())
            {
                var isLowerTrigram = ++i <= TrigramInfo.TotalLines;
                var origin = new Point(strokeThickness / 2, y);

                if (line.IsYang)
                {
                    render(isLowerTrigram, new Rect(origin.X, origin.Y, lineSize.Width, lineSize.Height));
                }
                else
                {
                    render(isLowerTrigram, new Rect(origin, yinSegmentSize));
                    render(isLowerTrigram, new Rect(new Point(origin.X + lineSize.Width - yinSegmentSize.Width, origin.Y), yinSegmentSize));
                }

                y -= lineHeight * (1 + spacer);
            }
        }
    }
}
