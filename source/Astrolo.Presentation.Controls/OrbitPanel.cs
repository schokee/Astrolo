using Astrolo.Geometry;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Astrolo.Presentation.Controls
{
    public class OrbitPanel : Panel
    {
        private static readonly FrameworkPropertyMetadataOptions Options =
            //FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange |
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender;

        #region Dependency Properties

        public static readonly DependencyProperty AngleProperty = DependencyProperty.RegisterAttached(
            "Angle", typeof(double), typeof(OrbitPanel), new FrameworkPropertyMetadata(0.0, Options, Refresh));

        public static void SetAngle(DependencyObject o, double value)
        {
            o.SetValue(AngleProperty, value);
        }

        public static double GetAngle(DependencyObject o)
        {
            return (double)o.GetValue(AngleProperty);
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.RegisterAttached(
            "Offset", typeof(double), typeof(OrbitPanel), new FrameworkPropertyMetadata(0.0, Options, Refresh));

        public static double GetOffset(DependencyObject o)
        {
            return (double)o.GetValue(OffsetProperty);
        }

        public static void SetOffset(DependencyObject o, double value)
        {
            o.SetValue(OffsetProperty, value);
        }

        public static readonly DependencyProperty TranslateOnlyProperty = DependencyProperty.RegisterAttached(
            "TranslateOnly", typeof(bool), typeof(OrbitPanel), new FrameworkPropertyMetadata(default(bool), Options, Refresh));

        public static void SetTranslateOnly(UIElement uiElement, bool value)
        {
            uiElement.SetValue(TranslateOnlyProperty, value);
        }

        public static bool GetTranslateOnly(UIElement uiElement)
        {
            return (bool)uiElement.GetValue(TranslateOnlyProperty);
        }

        #endregion

        protected IEnumerable<UIElement> ChildElements => Children.OfType<UIElement>();

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in ChildElements)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            //SetValue(ConcentricPanel.ThicknessProperty, MaxThickness);
            return RingSize(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = RingSize(finalSize);
            var radius = (System.Math.Min(size.Width, size.Height) - MaxThickness) / 2;

            foreach (var child in ChildElements)
            {
                TransformChild(child, radius);
                ArrangeChild(child, size);
            }

            return size;
        }

        private Size RingSize(Size size)
        {
            var thickness = MaxThickness;
            return new Size(
                double.IsInfinity(size.Width) ? thickness : System.Math.Max(size.Width, thickness),
                double.IsInfinity(size.Height) ? thickness : System.Math.Max(size.Height, thickness));
        }

        private double MaxThickness => ChildElements
            .Select(child => child.DesiredSize.MaxExtent())
            .DefaultIfEmpty()
            .Max();

        private static void ArrangeChild(UIElement child, Size size)
        {
            var topLeft = new Point(
                (size.Width - child.DesiredSize.Width) / 2,
                (size.Height - child.DesiredSize.Height) / 2);

            child.Arrange(new Rect(topLeft, child.DesiredSize));
        }

        private static void TransformChild(UIElement child, double radius)
        {
            var offset = Angle.FromDegrees(GetOffset(child));

            if (GetTranslateOnly(child))
            {
                offset += Angle.FromDegrees(GetAngle(child));

                child.RenderTransform = new TranslateTransform
                {
                    X =  radius * offset.Sin(),
                    Y = -radius * offset.Cos()
                };
            }
            else
            {
                child.RenderTransform = new TransformGroup
                {
                    Children = new TransformCollection
                    {
                        new TranslateTransform
                        {
                            X =  radius * offset.Sin(),
                            Y = -radius * offset.Cos()
                        },
                        new RotateTransform { Angle = GetAngle(child) }
                    }
                };
            }

            child.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private static void Refresh(object o, DependencyPropertyChangedEventArgs e)
        {
            (o as FrameworkElement)?.Parent.InvalidateAndUpdateLayout();
        }
    }
}
