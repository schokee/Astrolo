using Astrolo.Explorer.Components;
using Astrolo.Geometry;
using Astrolo.YiJing;

namespace Astrolo.Explorer.UI.Visualisation.Hive;

public sealed class HexChartCell : Selectable, IFormattable
{
    public HexChartCell(int ring, HexagramFigure hexagram, Geometry.PointyTop.Point location, double radius)
    {
        Ring = ring;
        Hexagram = hexagram;
        Location = location;
        Radius = radius;
        Origin = Location.Scale(radius).ToCartesianPoint();
        IsSelected = true;
    }

    public int Ring { get; }

    public Geometry.PointyTop.Point Location { get; }

    public double Radius { get; }

    public HexagramFigure Hexagram { get;}

    public Geometry.Point Origin { get; }

    public uint Value => Hexagram.Ordinal;

    public override string ToString()
    {
        return FormatValue(Value, 8);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return string.IsNullOrEmpty(format)
            ? ToString()
            : format switch
            {
                "b" => FormatValue(Value >> 3, 2, 3) + "." + FormatValue(Value & 0x7, 2, 3),
                "o" => ToString(),
                _ => Value.ToString(format, formatProvider)
            };
    }

    private static string FormatValue(uint v, int toBase, int zeroPadding = 2)
    {
        return Convert.ToString(v, toBase).PadLeft(zeroPadding, '0');
    }
}
