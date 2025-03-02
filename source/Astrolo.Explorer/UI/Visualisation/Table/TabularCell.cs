using Astrolo.Explorer.Components;
using Astrolo.GeneKeys;
using Astrolo.YiJing;

namespace Astrolo.Explorer.UI.Visualisation.Table;

public class TabularCell : Selectable
{
    public TabularCell(IGeneKey key)
    {
        Key = key;
        IsSelected = true;
    }

    public IGeneKey Key { get; }

    public HexagramFigure Hexagram => Key.Hexagram;

    public override string ToString()
    {
        return Hexagram.ToString();
    }
}
