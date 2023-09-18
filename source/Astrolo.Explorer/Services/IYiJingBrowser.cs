using Astrolo.YiJing;

namespace Astrolo.Explorer.Services
{
    public interface IYiJingBrowser
    {
        void ViewInfo(HexagramFigure figure);

        void ViewReading(IEnumerable<Movement> changes);
    }
}
