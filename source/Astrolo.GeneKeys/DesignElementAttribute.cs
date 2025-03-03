using Astrolo.Astrology;

namespace Astrolo.GeneKeys;

[AttributeUsage(AttributeTargets.Field)]
public sealed class DesignElementAttribute(Marker marker, bool inDesign) : Attribute
{
    public Marker Marker { get; } = marker;

    public bool InDesign { get; } = inDesign;
}