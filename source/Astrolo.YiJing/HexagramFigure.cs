namespace Astrolo.YiJing
{
    public sealed class HexagramFigure : Figure, IFormattable
    {
        private readonly LookupInfo _lookup;

        internal delegate (IHexagramInfo Info, TrigramFigure UpperTrigram, TrigramFigure LowerTrigram) LookupInfo(uint ordinal);

        internal HexagramFigure(LookupInfo lookup, uint ordinal)
            : base(new BitArray(HexagramInfo.TotalLines, ordinal))
        {
            _lookup = lookup;

            var info = _lookup(ordinal);

            Info = info.Info;
            UpperTrigram = info.UpperTrigram;
            LowerTrigram = info.LowerTrigram;
        }

        public int Number => Info.Number;

        public string Description => Info.Title;

        public IHexagramInfo Info { get; }

        public TrigramFigure UpperTrigram { get; }

        public TrigramFigure LowerTrigram { get; }

        public bool IsYang => Bits.Count > TrigramInfo.TotalLines;

        public bool IsYin => Bits.Count < TrigramInfo.TotalLines;

        public bool IsBalanced => Bits.Count == TrigramInfo.TotalLines;

        public bool HasMirroredTrigrams => UpperTrigram.Bits.IsReverseOf(LowerTrigram.Bits);

        public bool HasMatchingTrigrams => UpperTrigram.Ordinal == LowerTrigram.Ordinal;

        public bool HasTrigram(Trigram trigram)
        {
            return LowerTrigram.Info.Key == trigram || UpperTrigram.Info.Key == trigram;
        }

        public HexagramFigure Complement => new(_lookup, Bits.Complement);

        public HexagramFigure Invert => new(_lookup, Bits.Reversed);

        public IEnumerable<Movement> Diff(HexagramFigure other)
        {
            return this.Zip(other ?? throw new ArgumentNullException(nameof(other)), (a, b) => new Movement(a, b));
        }

        public override string ToString()
        {
            return $"{Number} {Description}";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return format switch
            {
                "n" => Description,
                "b" => FormatValue(Ordinal >> 3, 2, 3) + "." + FormatValue(Ordinal & 0x7, 2, 3),
                "t" => $"{UpperTrigram.Info.Key} over {LowerTrigram.Info.Key}",
                _ => ToString()
            };
        }

        public static bool IsValidNumber(int number)
        {
            return number is > 0 and <= HexagramInfo.TotalHexagrams;
        }

        private static string FormatValue(uint v, int toBase, int zeroPadding = 2)
        {
            return Convert.ToString(v, toBase).PadLeft(zeroPadding, '0');
        }
    }
}
