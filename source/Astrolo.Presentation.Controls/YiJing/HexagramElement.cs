using Astrolo.YiJing;

namespace Astrolo.Presentation.Controls.YiJing
{
    public class HexagramElement : FrameworkElement
    {
        private Brush? UpperBrush { get; set; }
        private Brush? LowerBrush { get; set; }

        public static readonly DependencyProperty HexagramProperty = DependencyProperty.Register(
            nameof(Hexagram), typeof(HexagramFigure), typeof(HexagramElement),
            new FrameworkPropertyMetadata(default(HexagramFigure), FrameworkPropertyMetadataOptions.AffectsRender, (s, _) => ((HexagramElement)s).OnValueChanged()));

        public HexagramFigure? Hexagram
        {
            get => (HexagramFigure)GetValue(HexagramProperty);
            set => SetValue(HexagramProperty, value);
        }

        private void OnValueChanged()
        {
            if (Hexagram is not null)
            {
                UpperBrush = new SolidColorBrush(Hexagram.UpperTrigram.GetColor());
                LowerBrush = new SolidColorBrush(Hexagram.LowerTrigram.GetColor());
            }

            InvalidateVisual();
        }

        public static readonly DependencyProperty HexagramAspectProperty = DependencyProperty.Register(
            nameof(HexagramAspect), typeof(HexagramAspect), typeof(HexagramElement),
            new FrameworkPropertyMetadata(HexagramAspect.Hexagram, FrameworkPropertyMetadataOptions.AffectsRender, (s, _) => ((HexagramElement)s).InvalidateVisual()));

        public HexagramAspect HexagramAspect
        {
            get => (HexagramAspect)GetValue(HexagramAspectProperty);
            set => SetValue(HexagramAspectProperty, value);
        }

        #region SpacerProperty

        public static readonly DependencyProperty SpacerProperty = DependencyProperty.Register(nameof(Spacer), typeof(float),
            typeof(HexagramElement), new UIPropertyMetadata(0.6f, OnSpacerChanged, CoerceSpacer));

        public float Spacer
        {
            get => (float)GetValue(SpacerProperty);
            set => SetValue(SpacerProperty, value);
        }

        private static void OnSpacerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HexagramElement)d).InvalidateVisual();
        }

        private static object CoerceSpacer(DependencyObject d, object baseValue)
        {
            return Math.Min(Math.Max(0, Convert.ToSingle(baseValue)), 1);
        }

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (Hexagram == null)
                return;

            HexagramShape.Render(Hexagram, new Size(ActualWidth, ActualHeight), Spacer, 0, (isLowerTrigram, bounds) =>
            {
                if ((HexagramAspect & (isLowerTrigram ? HexagramAspect.LowerTrigram : HexagramAspect.UpperTrigram)) != 0)
                {
                    drawingContext.DrawRectangle(isLowerTrigram ? LowerBrush : UpperBrush, null, bounds);
                }
            });
        }
    }
}
