using CsvHelper.Configuration.Attributes;

namespace Astrolo.YiJing
{
    public class HexagramInfo : IHexagramInfo
    {
        public const int TotalLines = 6;

        public const int TotalHexagrams = 1 << TotalLines;

        public int Number { get; set; }

        public string Title { get; set; }

        public Trigram LowerTrigram { get; set; }

        public Trigram UpperTrigram { get; set; }

        [Ignore]
        public YiSpherePoint SphereLocation { get; set; }
    }
}
