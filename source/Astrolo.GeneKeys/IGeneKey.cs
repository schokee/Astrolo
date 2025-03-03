using Astrolo.HumanDesign;
using Astrolo.YiJing;

namespace Astrolo.GeneKeys;

public interface IGeneKey : IComparable, IComparable<IGeneKey>
{
    IGeneKey ProgrammingPartner { get; }

    int Number { get; }

    string Siddhi { get; }
    string Gift { get; }
    string Shadow { get; }

    string Repressive { get; }
    string Reactive { get; }
    string Dilemma { get; }
    string VictimState { get; }

    HexagramFigure Hexagram { get; }

    IGateInfo Gate { get; }

    Uri InfoLink { get; }

    ICodonRing CodonRing { get; }
}