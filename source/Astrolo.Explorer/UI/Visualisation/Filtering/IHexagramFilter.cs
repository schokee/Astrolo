using Astrolo.Explorer.Components;
using Astrolo.YiJing;

namespace Astrolo.Explorer.UI.Visualisation.Filtering;

[Flags]
public enum VisualStates
{
    None,
    Selected = 1,
    Emphasized = 3
}

public interface IHexagramFilter : ISelectable
{
    VisualStates GetState(HexagramFigure hexagram);
}
