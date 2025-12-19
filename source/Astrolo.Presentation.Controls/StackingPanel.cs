namespace Astrolo.Presentation.Controls
{
    public class StackingPanel : Panel
    {
        #region SpacerProperty

        public static readonly DependencyProperty SpacerProperty = DependencyProperty.Register(nameof(Spacer), typeof(float),
            typeof(StackingPanel), new UIPropertyMetadata(0.6f, OnSpacerChanged, CoerceSpacer));

        public float Spacer
        {
            get => (float)GetValue(SpacerProperty);
            set => SetValue(SpacerProperty, value);
        }

        private static void OnSpacerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StackingPanel)d).InvalidateVisual();
        }

        private static object CoerceSpacer(DependencyObject d, object baseValue)
        {
            return Math.Min(Math.Max(0, Convert.ToSingle(baseValue)), 1);
        }

        #endregion

        protected override Size MeasureOverride(Size constraint)
        {
            return constraint;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (Children.Count <= 0)
            {
                return arrangeSize;
            }

            var lineHeight = ActualHeight / (Children.Count + (Children.Count - 1) * Spacer);
            var spacer = lineHeight * Spacer;

            var y = ActualHeight - lineHeight;

            var index = 0;
            for (var count = Children.Count; index < count; ++index)
            {
                var uiElement = Children[index];
                if (uiElement == null)
                {
                    continue;
                }

                var bounds = new Rect(0, y, arrangeSize.Width, lineHeight);
                uiElement.Arrange(bounds);

                y -= lineHeight + spacer;
            }

            return arrangeSize;
        }
    }
}
