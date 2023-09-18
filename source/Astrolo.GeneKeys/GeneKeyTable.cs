using Astrolo.Core;
using Astrolo.GeneKeys.Metadata;
using Astrolo.HumanDesign;
using Astrolo.YiJing;

namespace Astrolo.GeneKeys
{
    public sealed class GeneKeyTable : IEnumerable<IGeneKey>
    {
        private IReadOnlyDictionary<uint, GeneKey> Lookup { get; }

        public static IReadOnlyList<Sphere> AllSpheres { get; } = Enum.GetValues(typeof(Sphere)).Cast<Sphere>().ToList();

        public static IReadOnlyList<Sphere> VenusSpheres => AllSpheres.Skip(3).Take(6).ToList();

        public GeneKeyTable(IGateDictionary allGates)
        {
            var metadata = EmbeddedFile.DeserializeCsv<FrequencyInfo>("Frequencies.csv");

            Gates = allGates;
            Lookup = allGates.Values
                .Join(metadata, x => x.Number, x => x.Number, (gate, info) => new GeneKey(this, gate, info))
                .ToSortedList(x => (uint)x.Number);
        }

        public GeneKeyTable() : this(GateDictionary.Create())
        {
        }

        public IGateDictionary Gates { get; }

        public bool IsIndexInRange(int number)
        {
            return number > 0 && number <= Lookup.Count;
        }

        public IGeneKey this[int number] => Lookup[(uint)number];

        public IEnumerator<IGeneKey> GetEnumerator()
        {
            return Lookup.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class GeneKey : IGeneKey, IFormattable
        {
            private GeneKeyTable Directory { get; }

            public GeneKey(GeneKeyTable directory, IGateInfo gate, FrequencyInfo info)
            {
                Directory = directory;
                Gate = gate;
                Siddhi = info.Siddhi;
                Gift = info.Gift;
                Shadow = info.Shadow;
                Repressive = info.Repressive;
                Reactive = info.Reactive;
                Dilemma = info.Dilemma;
                VictimState = info.VictimState;
                CodonRing = CodonRingTable.LookupById(Number);
            }

            public int Number => Gate.Number;
            public IGeneKey ProgrammingPartner => Directory.Lookup.Values.First(x => x.Hexagram.Ordinal == Hexagram.Bits.Complement.Value);

            public string Siddhi { get; }
            public string Gift { get; }
            public string Shadow { get; }

            public string Repressive { get; }
            public string Reactive { get; }

            public string Dilemma { get; }
            public string VictimState { get; }

            public Uri InfoLink => new($"https://genekeys.com/gene-key-{Number}/");

            public IGateInfo Gate { get; }
            public HexagramFigure Hexagram => Gate.Hexagram;
            public ICodonRing CodonRing { get; }

            public override string ToString()
            {
                return Number.ToString();
            }

            public string ToString(string? format, IFormatProvider? formatProvider)
            {
                return format switch
                {
                    "F" => $"{Number} - {Siddhi}",
                    _ => Number.ToString()
                };
            }

            public int CompareTo(IGeneKey? other)
            {
                return Number.CompareTo(other?.Number);
            }

            public int CompareTo(object? obj)
            {
                return CompareTo(obj as IGeneKey);
            }
        }

        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private struct FrequencyInfo
        {
            public int Number { get; set; }
            public string Siddhi { get; set; }
            public string Gift { get; set; }
            public string Shadow { get; set; }
            public string Repressive { get; set; }
            public string Reactive { get; set; }
            public string Dilemma { get; set; }
            public string VictimState { get; set; }
        }
    }
}
