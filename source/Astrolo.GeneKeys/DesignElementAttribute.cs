using Astrolo.Astrology;

namespace Astrolo.GeneKeys
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DesignElementAttribute : Attribute
    {
        public DesignElementAttribute(Marker marker, bool inDesign)
        {
            Marker = marker;
            InDesign = inDesign;
        }

        public Marker Marker { get; }

        public bool InDesign { get; }
    }
}
