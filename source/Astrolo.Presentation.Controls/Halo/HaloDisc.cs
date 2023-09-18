using Astrolo.Geometry;

namespace Astrolo.Presentation.Controls.Halo
{
    public class HaloDisc : HaloShape
    {
        private readonly EllipseGeometry _ellipse = new();

        public HaloDisc()
        {
            DefiningGeometry = _ellipse;
        }

        protected override System.Windows.Media.Geometry DefiningGeometry { get; }

        protected override void Arrange(Circle circle)
        {
            _ellipse.Center = circle.Center.ToPoint();
            _ellipse.RadiusX = circle.Radius;
            _ellipse.RadiusY = circle.Radius;
        }
    }
}
