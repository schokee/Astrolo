namespace Astrolo.Presentation.Controls.GeneKeys
{
    public class WingPanel : Canvas
    {
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached(
            "Placement", typeof(WingPlacement), typeof(WingPanel), new PropertyMetadata(default(WingPlacement)));

        public static void SetPlacement(DependencyObject element, WingPlacement value)
        {
            element.SetValue(PlacementProperty, value);
        }

        public static WingPlacement GetPlacement(DependencyObject element)
        {
            return (WingPlacement)element.GetValue(PlacementProperty);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in Children)
            {
                var placement = GetPlacement(element);
                var elementSize = element.DesiredSize;

                switch (placement)
                {
                    case WingPlacement.Left:
                        SetLeft(element, -elementSize.Width);
                        break;
                    case WingPlacement.Right:
                        SetLeft(element, finalSize.Width);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return base.ArrangeOverride(finalSize);
        }
    }
}
