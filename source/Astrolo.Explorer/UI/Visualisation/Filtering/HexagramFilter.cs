using System.Globalization;
using Astrolo.Astrology;
using Astrolo.Core;
using Astrolo.Explorer.Model;
using Astrolo.GeneKeys;
using Astrolo.HumanDesign;
using Astrolo.YiJing;
using MoreLinq;

namespace Astrolo.Explorer.UI.Visualisation.Filtering
{
    public class HexagramFilter : Filter<HexagramFigure>, IHexagramFilter
    {
        private static Lazy<IGateDictionary> GateDictionary { get; } = new(HumanDesign.GateDictionary.Create);

        public static HexagramFilter Null { get; } = new("All");

        public static IEnumerable<HexagramFilter> Basic
        {
            get
            {
                yield return new HexagramFilter("Primary", x => x.HasMatchingTrigrams);
                yield return new HexagramFilter("Mirrored", x => x.HasMirroredTrigrams);
                yield return new HexagramFilter("Surface", x => !x.Info.SphereLocation.Inside);
                yield return new HexagramFilter("Core", x => x.Info.SphereLocation.Inside);
            }
        }

        public static IEnumerable<HexagramFilter> YinYang
        {
            get
            {
                const string yinYangCategory = "Yin/Yang";

                for (var i = HexagramInfo.TotalLines; i >= 0; --i)
                {
                    var yangCount = i;
                    yield return new HexagramFilter($"{HexagramInfo.TotalLines - yangCount} Yin / {yangCount} Yang", x => x.Bits.Count == yangCount) { Category = yinYangCategory };
                }

                yield return new HexagramFilter("Yang", x => x.IsYang) { Category = yinYangCategory };
                yield return new HexagramFilter("Yin", x => x.IsYin) { Category = yinYangCategory };

                yield return new HexagramFilter("Upper Yang", x => x.UpperTrigram.IsYang) { Category = yinYangCategory };
                yield return new HexagramFilter("Upper Yin", x => x.UpperTrigram.IsYin) { Category = yinYangCategory };

                yield return new HexagramFilter("Lower Yang", x => x.LowerTrigram.IsYang) { Category = yinYangCategory };
                yield return new HexagramFilter("Lower Yin", x => x.LowerTrigram.IsYin) { Category = yinYangCategory };
            }
        }

        public static IEnumerable<HexagramFilter> TrigramPairs
        {
            get
            {
                HexagramFilter CreateTrigramFilter(params Trigram[] trigrams)
                {
                    var label = string.Join(" / ", trigrams.Select(x => x.GetDescriptionOrDefault()));
                    return new HexagramFilter(label, x => trigrams.Any(x.HasTrigram)) { Category = "Trigram Combinations" };
                }

                yield return CreateTrigramFilter(Trigram.Heaven, Trigram.Earth);
                yield return CreateTrigramFilter(Trigram.Thunder, Trigram.Wind);
                yield return CreateTrigramFilter(Trigram.Mountain, Trigram.Lake);
                yield return CreateTrigramFilter(Trigram.Fire, Trigram.Water);
            }
        }

        public static IEnumerable<HexagramFilter> Trigrams => Enum.GetValues(typeof(Trigram)).Cast<Trigram>()
            .OrderBy(x => x.ToBitArray().Reversed.Value)
            .Select(trigram => new HexagramFilter(trigram.GetDescriptionOrDefault(), x => x.HasTrigram(trigram))
            {
                Category = "Trigram"
            });

        public static IEnumerable<HexagramFilter> CodonRings => CodonRingTable.All
            .Select(ring => new HexagramFilter(ring.ToString("R", CultureInfo.CurrentCulture), x => ring.Sequence.Contains(x.Number))
            {
                Category = "Codon Ring"
            });

        public static IEnumerable<HexagramFilter> Elements => Enum.GetValues(typeof(Element)).Cast<Element>()
            .OrderBy(x => x.ToString())
            .GroupJoin(GateDictionary.Value.SelectMany(x => x.Value.Lines), x => x, x => x.MandalaSlice.SignInfo.Element, (el, matching) =>
            {
                var lookup = MoreEnumerable.ToHashSet(matching.Select(x => x.Gate.Number));
                return new HexagramFilter(el.ToString(), x => lookup.Contains(x.Number)) { Category = "Element" };
            });

        public static IEnumerable<HexagramFilter> ZodiacSigns => Zodiac.AllSigns
            .Select(x => x.Sign)
            .GroupJoin(GateDictionary.Value.SelectMany(x => x.Value.Lines), x => x, x => x.MandalaSlice.Sign, (sign, matching) =>
            {
                var lookup = MoreEnumerable.ToHashSet(matching.Select(x => x.Gate.Number));
                return new HexagramFilter(sign.ToString(), x => lookup.Contains(x.Number)) { Category = "Zodiac" };
            });

        private static ISet<int> DeltaPath { get; } = new HashSet<int>(new[] { 22, 12, 6, 59, 27, 50, 57, 20 });

        public static IEnumerable<HexagramFilter> GeneKeys
        {
            get
            {
                yield return new HexagramFilter("Delta", x => DeltaPath.Contains(x.Number)) { Category = "Gene Keys" };
            }
        }

        public static IEnumerable<HexagramFilter> Centers => GateDictionary.Value.Values
            .GroupBy(x => x.Center, x => x.Number)
            .OrderBy(x => x.Key)
            .Select(grouping => new HexagramFilter(grouping.Key.GetMnemonicOrDefault(), figure => grouping.Contains(figure.Number)) { Category = "Center" });

        public static IEnumerable<HexagramFilter> IncarnationCrosses => IncarnationCross
            .Load()
            .DistinctBy(x => x.PartialName)
            .Select(x => new HexagramFilter(x.ToString("l", CultureInfo.CurrentCulture), figure => x.Gates.Contains(figure.Number)) { Category = "Incarnation Cross" });

        public static IEnumerable<HexagramFilter> All => Basic
            .Concat(YinYang)
            .Concat(Trigrams)
            .Concat(GeneKeys)
            //.Concat(TrigramPairs)
            .Concat(CodonRings)
            .Concat(Elements)
            .Concat(ZodiacSigns)
            .Concat(Centers)
            .Concat(IncarnationCrosses);

        protected HexagramFilter(string label, Func<HexagramFigure, bool> predicate = null) : base(label, predicate)
        {
        }

        public string Category { get; set; } = "General";

        public override bool Includes(HexagramFigure figure)
        {
            return figure is not null && base.Includes(figure);
        }
    }
}
