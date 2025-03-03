using Astrolo.Core;

namespace Astrolo.YiJing;

public sealed class TrigramInfo(Trigram key, string name, string direction)
{
    public const int TotalLines = 3;

    public Trigram Key { get; } = key;

    public BitArray Bits { get; } = key.ToBitArray();

    public string Name { get; } = name;

    public string Direction { get; } = direction;

    public override string ToString()
    {
        return $"{Name}, {Key.GetDescriptionOrDefault()}";
    }
}
