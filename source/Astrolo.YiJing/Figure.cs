using System.Collections;

namespace Astrolo.YiJing
{
    public abstract class Figure : IFigure
    {
        internal Figure(BitArray bits)
        {
            Bits = bits;
        }

        public uint Ordinal => Bits.Value;

        public int LineCount => Bits.Length;

        public Line this[int index] => Bits[LineCount - index - 1];

        public IEnumerator<Line> GetEnumerator()
        {
            // Lines are returned bottom-up.
            return Bits.Reverse().Select(x => (Line)x).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public BitArray Bits { get; }
    }
}
