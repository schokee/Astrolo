using Astrolo.YiJing;
using Sprache;

namespace Astrolo.GeneKeys
{
    public static class Parser
    {
        private static Parser<char> NonZeroDigit =>
            Parse.Char(x => char.IsDigit(x) && x > '0', "non-zero");

        private static Parser<char> Delimiter =>
            Parse.Char(c => c is ',' or ';', "delimiter").Token();

        private static Parser<int> Int => Parse.Number.Select(int.Parse);

        private static Parser<int> Key => Int.Where(x => x is > 0 and <= HexagramInfo.TotalHexagrams);

        private static Parser<int> Line => Int.Where(x => x is > 0 and <= HexagramInfo.TotalLines);

        private static Parser<(int Number, int Line)> NumberWithLine =>
            from number in Key
            from line in Parse.Char('.').Then(_ => Line)
            select (number, line);

        public static IEnumerable<(int Number, int Line)> ParseSequence(string text, int length = 11)
        {
            var parser = NumberWithLine.DelimitedBy(Delimiter, length, length);
            return parser.Parse(text);
        }
    }
}
