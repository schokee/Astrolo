using Astrolo.Core;
using Astrolo.YiJing;

namespace Astrolo.GeneKeys
{
    public static class CodonRingTable
    {
        static CodonRingTable()
        {
#if DEBUG
            var isOk = All.SelectMany(x => x.Sequence).OrderBy(x => x).SequenceEqual(Enumerable.Range(1, HexagramInfo.TotalHexagrams));
#endif
        }

        public static ICodonRing LookupById(int id)
        {
            return All.First(x => x.Sequence.Contains(id));
        }

        public static ICodonRing[] All { get; } =
        {
            new CodonRingInfo(CodonRing.Fire, "Temperance", 1, 14),
            new CodonRingInfo(CodonRing.Water, "The Star", 2, 8),
            new CodonRingInfo(CodonRing.LifeAndDeath, "Death", 3, 20, 23, 24, 27, 42),
            new CodonRingInfo(CodonRing.Union, "The Lovers", 4, 7, 29, 59),
            new CodonRingInfo(CodonRing.Light, "The Sun", 5, 9, 11, 26),
            new CodonRingInfo(CodonRing.Alchemy, "The Magician", 6, 40, 47, 64),
            new CodonRingInfo(CodonRing.Humanity, "The Chariot", 10, 17, 21, 25, 38, 51),
            new CodonRingInfo(CodonRing.Trials, "The Hanged Man", 12, 33, 56),
            //new CodonRingInfo(CodonRing.Secrets, "The High Priestess", 12),
            new CodonRingInfo(CodonRing.Purification, "The Devil", 13, 30),
            new CodonRingInfo(CodonRing.Seeking, "The Hermit", 15, 39, 52, 53, 54, 58),
            new CodonRingInfo(CodonRing.Prosperity, "Prosperity", 16, 45),
            new CodonRingInfo(CodonRing.Matter, "The Emperor", 18, 46, 48, 57),
            new CodonRingInfo(CodonRing.Gaia, "The Empress", 19, 60, 61),
            new CodonRingInfo(CodonRing.Divinity, "The Hierophant", 22, 36, 37, 63),
            new CodonRingInfo(CodonRing.Illusion, "The Moon", 28, 32),
            new CodonRingInfo(CodonRing.NoReturn, "Judgement", 31, 62),
            new CodonRingInfo(CodonRing.Destiny, "The Wheel of Fortune", 34, 43),
            new CodonRingInfo(CodonRing.Miracles, "Strength", 35),
            new CodonRingInfo(CodonRing.Origin, "The Fool", 41),
            new CodonRingInfo(CodonRing.Illuminati, "Justice", 44, 50),
            new CodonRingInfo(CodonRing.TheWhirlwind, "The Tower", 49, 55)
        };

        private class CodonRingInfo : ICodonRing
        {
            public CodonRingInfo(CodonRing id, string arcanaArchetype, params int[] sequence)
            {
                Id = id;
                Sequence = sequence;
                ArcanaArchetype = arcanaArchetype;
            }

            public CodonRing Id { get; }

            public string Name => Id.GetDescriptionOrDefault();

            public string ArcanaArchetype { get; }

            public IReadOnlyList<int> Sequence { get; }

            public override string ToString()
            {
                return $"The Ring of {Name} ({string.Join(", ", Sequence)})";
            }

            public string ToString(string? format, IFormatProvider? formatProvider)
            {
                return format switch
                {
                    "F" => ToString(),
                    "S" => string.Join(", ", Sequence),
                    "R" => $"The Ring of {Name}",
                    _ => Name
                };
            }
        }
    }
}
