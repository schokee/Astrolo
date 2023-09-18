namespace Astrolo.HumanDesign
{
    public sealed partial class GateDictionary
    {
        private sealed class Channel : IChannel, IEquatable<Channel>, IFormattable
        {
            public Channel(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public IGateInfo StartGate => Gates[0];
            public IGateInfo EndGate => Gates[1];

            public List<IGateInfo> Gates { get; } = new();

            public bool Equals(Channel? other)
            {
                if (other == null) return false;
                return ReferenceEquals(this, other) || Gates.SequenceEqual(other.Gates);
            }

            public override bool Equals(object? obj)
            {
                return Equals(obj as Channel);
            }

            public override int GetHashCode()
            {
                return Gates.GetHashCode();
            }

            public string ToString(string? format, IFormatProvider? formatProvider)
            {
                return format switch
                {
                    "g" => Name,
                    "c" => $"{StartGate.Number:00}{EndGate.Number:00} {Name}",
                    _ => ToString()
                };
            }

            public override string ToString()
            {
                return $"{Name} ({StartGate.Number}-{EndGate.Number})";
            }

            public IEnumerator<IGateInfo> GetEnumerator()
            {
                return Gates.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
