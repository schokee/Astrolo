using Astrolo.Geometry;
using Size = System.Windows.Size;

namespace Astrolo.Presentation.Controls.Halo
{
    public abstract class HaloShape : Shape
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Arrange(Circle.CenterWithin(new Geometry.Size(finalSize.Width, finalSize.Height)).ReduceBy(StrokeThickness / 2));
            return base.ArrangeOverride(finalSize);
        }

        protected abstract void Arrange(Circle circle);
    }
}
