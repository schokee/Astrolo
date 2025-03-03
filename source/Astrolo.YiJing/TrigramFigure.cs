namespace Astrolo.YiJing;

public sealed class TrigramFigure(TrigramInfo info) : Figure(info.Bits), IPolarized
{
    public bool IsYang => Info.Bits.Count > 1;

    public bool IsYin => !IsYang;

    public TrigramInfo Info { get; } = info;

    public override string ToString()
    {
        return Info.ToString();
    }

    internal BitArray Over(TrigramFigure other)
    {
        return Bits.Append(other.Bits);
    }
}
