using Astrolo.Core;

namespace Astrolo.YiJing
{
    public sealed class TrigramInfo
    {
        public const int TotalLines = 3;

        public TrigramInfo(Trigram key, string name, string direction)
        {
            Key = key;
            Bits = key.ToBitArray();
            Name = name;
            Direction = direction;
        }

        public Trigram Key { get; }

        public BitArray Bits { get; }

        public string Name { get; }

        public string Direction { get; }

        public override string ToString()
        {
            return $"{Name}, {Key.GetDescriptionOrDefault()}";
        }
    }
}
