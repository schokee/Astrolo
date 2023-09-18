using System.ComponentModel;

namespace Astrolo.Astrology
{
    public enum Marker
    {
        Sun,
        Moon,
        Mercury,
        Venus,
        Earth,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Pluto,
        [Description("North Node")]
        NorthNode,
        [Description("South Node")]
        SouthNode
    }
}
