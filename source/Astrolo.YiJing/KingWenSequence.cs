using Astrolo.YiJing.Metadata;

namespace Astrolo.YiJing
{
    public sealed class KingWenSequence : ISequence
    {
        private static readonly IReadOnlyList<TrigramFigure> OrderedTrigrams;
        private static readonly IReadOnlyDictionary<Trigram, TrigramFigure> TrigramFigures;

        static KingWenSequence()
        {
            OrderedTrigrams = new TrigramInfo[]
                {
                    new(Trigram.Earth, "The Receptive", "North"),
                    new(Trigram.Mountain, "Keeping Still", "Northwest"),
                    new(Trigram.Water, "The Abysmal", "West"),
                    new(Trigram.Wind, "The Gentle", "Northeast"),
                    new(Trigram.Thunder, "The Arousing", "Southeast"),
                    new(Trigram.Fire, "The Clinging", "East"),
                    new(Trigram.Lake, "The Joyous", "Southwest"),
                    new(Trigram.Heaven, "The Creative", "South")
                }
                .Select(info => new TrigramFigure(info))
                .ToList();

            TrigramFigures = OrderedTrigrams.ToDictionary(x => x.Info.Key);
        }

        public KingWenSequence()
        {
            var points = YiSpherePoint.EnumeratePoints();

            var lookupByOrdinal = EmbeddedFile
                .DeserializeCsv<HexagramInfo>("KingWen.csv")
                .Select(info =>
                {
                    var upperTrigram = TrigramFigures[info.UpperTrigram];
                    var lowerTrigram = TrigramFigures[info.LowerTrigram];
                    var ordinal = upperTrigram.Over(lowerTrigram).Value;

                    info.SphereLocation = points[ordinal];

                    return (Key: ordinal, Value: (info, upperTrigram, lowerTrigram));
                })
                .ToDictionary(x => x.Key, x => x.Value);

            Hexagrams = lookupByOrdinal
                .Select(x => new HexagramFigure(ordinal => lookupByOrdinal[ordinal], x.Key))
                .OrderBy(x => x.Ordinal)
                .ToList();
        }

        public string Name => "King Wen";

        public IReadOnlyList<TrigramFigure> Trigrams => OrderedTrigrams;

        public IReadOnlyList<HexagramFigure> Hexagrams { get; }

        public HexagramFigure this[Trigram lower, Trigram upper]
        {
            get
            {
                var ordinal = TrigramFigures[upper].Over(TrigramFigures[lower]);
                return Hexagrams.First(x => x.Ordinal == ordinal);
            }
        }

        public HexagramFigure[][] ToTable()
        {
            var trigrams = OrderedTrigrams.Select(x => x.Info.Key).ToArray();

            return trigrams
                .Select(lower => trigrams.Select(upper => this[lower, upper]).ToArray())
                .ToArray();
        }
    }
}
