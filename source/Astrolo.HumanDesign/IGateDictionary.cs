using Astrolo.YiJing;

namespace Astrolo.HumanDesign
{
    public interface IGateDictionary : IReadOnlyDictionary<int, IGateInfo>
    {
        ILineInfo this[LineOfHexagram lineOfHexagram] { get; }

        ILineInfo this[int gate, int line] { get; }

        IEnumerable<IGateInfo> InTransitOrder { get; }
    }
}
