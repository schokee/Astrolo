using Astrolo.Core;
using Astrolo.YiJing;

namespace Astrolo.HumanDesign;

public sealed partial class GateDictionary : IGateDictionary
{
    private IReadOnlyDictionary<int, IGateInfo> AllGates { get; }

    private GateDictionary(IEnumerable<IGateInfo> allGates)
    {
        AllGates = allGates.ToSortedList(x => x.Number);
    }

    public int Count => AllGates.Count;

    public IEnumerable<int> Keys => AllGates.Keys;

    public IEnumerable<IGateInfo> Values => AllGates.Values;

    public IEnumerable<IGateInfo> InTransitOrder => Values.OrderByDescending(x => (double)x.MandalaSlice.StartAngle);

    public IGateInfo this[int key] => AllGates[key];

    public ILineInfo this[LineOfHexagram lineOfHexagram] => this[lineOfHexagram.Hexagram, lineOfHexagram.Line];

    public ILineInfo this[int gate, int line] => AllGates[gate][line];

    public bool ContainsKey(int key)
    {
        return AllGates.ContainsKey(key);
    }

    public bool TryGetValue(int key, out IGateInfo value)
    {
#pragma warning disable CS8601
        return AllGates.TryGetValue(key, out value);
#pragma warning restore CS8601
    }

    public IEnumerator<KeyValuePair<int, IGateInfo>> GetEnumerator()
    {
        return AllGates.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Orders hexagrams in transit (HD mandala) order
    /// </summary>
    public sealed class HexagramComparer : IComparer<HexagramFigure>
    {
        public int Compare(HexagramFigure? x, HexagramFigure? y)
        {
            var diff = GetLowerTrigramOrdinal(y!) - GetLowerTrigramOrdinal(x!);
            if (diff != 0)
            {
                return diff;
            }

            return GetUpperTrigramOrdinal(y!) - GetUpperTrigramOrdinal(x!);
        }

        private static int GetLowerTrigramOrdinal(HexagramFigure figure)
        {
            var trigram = figure.LowerTrigram.Info.Key;

            return trigram switch
            {
                Trigram.Heaven => 0,
                Trigram.Wind => 1,
                Trigram.Water => 2,
                Trigram.Mountain => 3,
                Trigram.Earth => 4,
                Trigram.Thunder => 5,
                Trigram.Fire => 6,
                Trigram.Lake => 7,
                _ => throw new ArgumentOutOfRangeException(nameof(TrigramInfo.Key), trigram, null)
            };
        }

        private static int GetUpperTrigramOrdinal(HexagramFigure figure)
        {
            static int GetOrdinal(Trigram trigram)
            {
                return trigram switch
                {
                    Trigram.Heaven => 0,
                    Trigram.Lake => 1,
                    Trigram.Fire => 2,
                    Trigram.Thunder => 3,
                    Trigram.Wind => 4,
                    Trigram.Water => 5,
                    Trigram.Mountain => 6,
                    Trigram.Earth => 7,
                    _ => throw new ArgumentOutOfRangeException(nameof(TrigramInfo.Key), trigram, null)
                };
            }

            var lowerTrigram = figure.LowerTrigram.Info.Key;
            var flip = lowerTrigram is Trigram.Thunder or Trigram.Fire or Trigram.Lake or Trigram.Heaven;

            var ordinal = GetOrdinal(figure.UpperTrigram.Info.Key);
            return flip ? 7 - ordinal : ordinal;
        }
    }
}
