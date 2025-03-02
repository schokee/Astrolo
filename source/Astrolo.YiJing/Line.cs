namespace Astrolo.YiJing;

public readonly struct Line : IPolarized, IEquatable<Line>
{
    public static Line Yang => true;

    public static Line Yin => false;

    private Line(bool isYang)
    {
        IsYang = isYang;
    }

    public bool IsYang { get; }

    public bool IsYin => !IsYang;

    public Line Complement => IsYang ? Yin : Yang;

    public bool Equals(Line other)
    {
        return IsYang == other.IsYang;
    }

    public override bool Equals(object obj)
    {
        return obj is Line other && Equals(other);
    }

    public override int GetHashCode()
    {
        return IsYang.GetHashCode();
    }

    public static bool operator ==(Line left, Line right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Line left, Line right)
    {
        return !left.Equals(right);
    }

    public static Line operator !(Line line)
    {
        return line.Complement;
    }

    public static implicit operator uint(Line line)
    {
        return line.IsYang ? 1u : 0;
    }

    public static implicit operator Line(bool isYang)
    {
        return new Line(isYang);
    }
}
