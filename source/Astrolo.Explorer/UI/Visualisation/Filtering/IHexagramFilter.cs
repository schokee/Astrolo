using Astrolo.Presentation.Core.Components;
using Astrolo.YiJing;

namespace Astrolo.Explorer.UI.Visualisation.Filtering
{
    public interface IHexagramFilter : ISelectable
    {
        bool Includes(HexagramFigure hexagram);
    }
}
