namespace Astrolo.Presentation.Controls
{
    public class SpacingPanel : StackPanel
    {
        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(nameof(Spacing), typeof(double),
            typeof(SpacingPanel), new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsMeasure), v => (double)v >= 0);

        [Bindable(true)]
        [Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        static SpacingPanel()
        {
            OrientationProperty.OverrideMetadata(typeof(SpacingPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var isHorizontal = Orientation == Orientation.Horizontal;

            var measuredSize = new Size();

            var index = 0;
            for (var final = Children.Count - 1; index <= final; ++index)
            {
                var uiElement = Children[index];
                if (uiElement == null) continue;

                uiElement.Measure(constraint);
                var desiredSize = uiElement.DesiredSize;

                if (isHorizontal)
                {
                    measuredSize.Height = Math.Max(measuredSize.Height, desiredSize.Height);
                    measuredSize.Width += index < final && desiredSize.Width > 0 ? desiredSize.Width + Spacing : desiredSize.Width;
                }
                else
                {
                    measuredSize.Width = Math.Max(measuredSize.Height, desiredSize.Height);
                    measuredSize.Height += index < final && desiredSize.Height > 0 ? desiredSize.Height + Spacing : desiredSize.Height;
                }
            }

            return new Size(
                Math.Min(measuredSize.Width, constraint.Width),
                Math.Min(measuredSize.Height, constraint.Height));
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var isHorizontal = Orientation == Orientation.Horizontal;

            var bounds = new Rect(arrangeSize);
            var spaceTaken = 0.0;

            var index = 0;
            for (var count = Children.Count; index < count; ++index)
            {
                var uiElement = Children[index];
                if (uiElement == null) continue;

                if (isHorizontal)
                {
                    bounds.X += spaceTaken;
                    bounds.Width = uiElement.DesiredSize.Width;
                    bounds.Height = Math.Max(arrangeSize.Height, uiElement.DesiredSize.Height);

                    spaceTaken = uiElement.DesiredSize.Width > 0 ? uiElement.DesiredSize.Width + Spacing : 0;
                }
                else
                {
                    bounds.Y += spaceTaken;
                    bounds.Height = uiElement.DesiredSize.Height;
                    bounds.Width = Math.Max(arrangeSize.Width, uiElement.DesiredSize.Width);

                    spaceTaken = uiElement.DesiredSize.Height > 0 ? uiElement.DesiredSize.Height + Spacing : 0;
                }

                uiElement.Arrange(bounds);
            }

            return arrangeSize;
        }
    }

}
