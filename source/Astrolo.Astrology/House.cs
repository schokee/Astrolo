using System.ComponentModel;

namespace Astrolo.Astrology;

public enum House
{
    Self = 1,
    Value,
    Sharing,
    [Description("Home and Family")]
    HomeAndFamily,
    Pleasure,
    Health,
    Balance,
    Transformation,
    Purpose,
    Enterprise,
    Blessings,
    Sacrifice
}
