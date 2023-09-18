using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Astrolo.YiJing
{
    public readonly struct LineOfHexagram : IComparable, IComparable<LineOfHexagram>, IEquatable<LineOfHexagram>, IXmlSerializable
    {
        public LineOfHexagram(int hexagram, int line)
        {
            if (hexagram is < 1 or > HexagramInfo.TotalHexagrams)
                throw new ArgumentOutOfRangeException(nameof(hexagram));

            if (line is < 1 or > HexagramInfo.TotalLines)
                throw new ArgumentOutOfRangeException(nameof(line));

            Hexagram = hexagram;
            Line = line;
        }

        public int Hexagram { get; }

        public int Line { get; }

        public int CompareTo(LineOfHexagram other)
        {
            var diff = Hexagram.CompareTo(other.Hexagram);
            return diff == 0 ? Line.CompareTo(other.Line) : diff;
        }

        public int CompareTo(object obj)
        {
            return ReferenceEquals(null, obj) ? 1 :
                obj is LineOfHexagram other ? CompareTo(other) :
                throw new ArgumentException($"Object must be of type {nameof(LineOfHexagram)}");
        }

        public bool Equals(LineOfHexagram other)
        {
            return Hexagram == other.Hexagram && Line == other.Line;
        }

        public override bool Equals(object obj)
        {
            return obj is LineOfHexagram other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            { return (Hexagram * 397) ^ Line; }
        }

        public override string ToString()
        {
            return $"{Hexagram}.{Line}";
        }

        private static Regex Pattern { get; } = new(@"^(?<Hexagram>(\d|[1-5]\d|6[0-4]))\.(?<line>[1-6])", RegexOptions.Singleline);

        public static LineOfHexagram Parse(string s)
        {
            return TryParse(s, out var result) ? result : throw new FormatException("Failed to parse: " + s);
        }

        public static bool TryParse(string s, out LineOfHexagram result)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                var match = Pattern.Match(s.Trim());
                if (match.Success)
                {
                    result = new LineOfHexagram(int.Parse(match.Groups["Hexagram"].Value), int.Parse(match.Groups["line"].Value));
                    return true;
                }
            }

            result = new LineOfHexagram();
            return false;
        }

        public static bool operator ==(LineOfHexagram left, LineOfHexagram right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LineOfHexagram left, LineOfHexagram right)
        {
            return !left.Equals(right);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Unsafe.AsRef(this) = Parse(reader.ReadElementContentAsString());
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString(ToString());
        }
    }
}
