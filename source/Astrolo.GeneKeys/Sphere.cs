using Astrolo.Astrology;

namespace Astrolo.GeneKeys
{
    // ReSharper disable InconsistentNaming
    public enum Sphere
    {
        [DesignElement(Marker.Sun, false), Mnemonic("LW"), Description("Life's Work")]
        LifesWork,
        [DesignElement(Marker.Earth, false), Mnemonic("EV")]
        Evolution,
        [DesignElement(Marker.Sun, true), Mnemonic("RA")]
        Radiance,
        [DesignElement(Marker.Earth, true), Mnemonic("PU")]
        Purpose,
        [DesignElement(Marker.Moon, true), Mnemonic("AT")]
        Attraction,
        [DesignElement(Marker.Venus, false)]
        IQ,
        [DesignElement(Marker.Mars, false)]
        EQ,
        [DesignElement(Marker.Venus, true)]
        SQ,
        [DesignElement(Marker.Mars, true), Mnemonic("VO")]
        Vocation,
        [DesignElement(Marker.Jupiter, true), Mnemonic("CU")]
        Culture,
        [DesignElement(Marker.Jupiter, false), Mnemonic("PE")]
        Pearl,

        [DesignElement(Marker.Saturn, true), Mnemonic("ST")]
        Stability,
        [DesignElement(Marker.Mercury, false), Mnemonic("RE")]
        Relating,
        [DesignElement(Marker.Uranus, true), Mnemonic("CR")]
        Creativity
    }
}
// ReSharper restore InconsistentNaming
