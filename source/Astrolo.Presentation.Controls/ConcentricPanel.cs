namespace Astrolo.Presentation.Controls
{
    public class ConcentricPanel : Panel
    {
        private static readonly FrameworkPropertyMetadataOptions Options =
            //FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange |
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender;

        #region Dependency Properties

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.RegisterAttached(
            "Thickness", typeof(double), typeof(ConcentricPanel),
            new FrameworkPropertyMetadata(0.0, Options));

        public static double GetThickness(DependencyObject o)
        {
            return (double)o.GetValue(ThicknessProperty);
        }

        public static void SetThickness(DependencyObject o, double value)
        {
            o.SetValue(ThicknessProperty, value);
        }

        public static readonly DependencyProperty BandProperty = DependencyProperty.RegisterAttached(
            "Band", typeof(int), typeof(ConcentricPanel),
            new FrameworkPropertyMetadata(0, Options));

        public static int GetBand(DependencyObject o)
        {
            return (int)o.GetValue(BandProperty);
        }

        public static void SetBand(DependencyObject o, int value)
        {
            o.SetValue(BandProperty, value);
        }

        #endregion

        #region UIElement Overrides

        protected override Size MeasureOverride(Size availableSize)
        {
            var area = new Rect(new Point(), availableSize);

            foreach (var grouping in OrderedChildren)
            {
                foreach (var child in grouping)
                {
                    child.Measure(new Size(area.Width, area.Height));
                }

                area = InnerArea(area, GetMaxThickness(grouping));
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var thickness = OrderedChildren.Sum(GetMaxThickness);

            var size = new Size(
                System.Math.Max(thickness * 2, finalSize.Width),
                System.Math.Max(thickness * 2, finalSize.Height));

            var area = new Rect(new Point(), size);

            foreach (var band in OrderedChildren)
            {
                foreach (var child in band)
                {
                    child.Arrange(area);
                }

                area = InnerArea(area, GetMaxThickness(band));
            }

            return size;
        }

        #endregion

        #region Private Members

        private IEnumerable<IGrouping<int, UIElement>> OrderedChildren => Children
            .Cast<UIElement>()
            .GroupBy(GetBand)
            .OrderBy(x => x.Key);

        private static double GetMaxThickness(IEnumerable<UIElement> band)
        {
            return band.Select(GetThickness).DefaultIfEmpty().Max();
        }

        private static Rect InnerArea(Rect area, double thickness)
        {
            return area.Width < thickness * 2 || area.Height < thickness * 2
                ? Rect.Empty
                : area.ReduceBy(thickness);
        }

        #endregion
    }
}
