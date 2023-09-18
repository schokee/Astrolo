namespace Astrolo.YiJing
{
    public sealed class TrigramFigure : Figure, IPolarized
    {
        public TrigramFigure(TrigramInfo info) : base(info.Bits)
        {
            Info = info;
        }

        public bool IsYang => Info.Bits.Count > 1;

        public bool IsYin => !IsYang;

        public TrigramInfo Info { get; }

        public override string ToString()
        {
            return Info.ToString();
        }

        internal BitArray Over(TrigramFigure other)
        {
            return Bits.Append(other.Bits);
        }
    }
}
