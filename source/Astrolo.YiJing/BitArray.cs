using System.Collections;

namespace Astrolo.YiJing
{
    public readonly struct BitArray : IEnumerable<bool>, IEquatable<BitArray>
    {
        public const int MaxLength = 8;

        private readonly byte _length;

        public BitArray(BitArray other)
            : this(other.Length, other.Value)
        {
        }

        public BitArray(int length, uint value)
            : this(length)
        {
            Value = TakeBits(length, value);
        }

        public BitArray(int length)
        {
            if (length is < 1 or > MaxLength) throw new ArgumentOutOfRangeException(nameof(length));

            _length = (byte)length;
            Value = 0;
        }

        public int Length => _length;

        public uint Value { get; }

        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
                return (Value & (1 << (Length - 1 - index))) != 0;
            }
        }

        public int Count => this.Count(x => x);

        public BitArray Empty => new(Length);

        public BitArray Complement => new(Length, ~Value);

        public BitArray Reversed => new(Length, (uint)this.Reverse().Aggregate(0, (result, x) => (result << 1) | (x ? 1 : 0)));

        public bool Equals(BitArray other)
        {
            return Length == other.Length && Value == other.Value;
        }

        public bool IsReverseOf(BitArray other)
        {
            var last = Length - 1;
            return Length == other.Length && this.Select((bit, n) => bit == other[last - n]).All(x => x);
        }

        public BitArray Append(BitArray other)
        {
            return new BitArray(Length + other.Length, (Value << Length) + other.Value);
        }

        public BitArray Difference(BitArray other)
        {
            return new BitArray(Length, Value & ~other.Value);
        }

        public override string ToString()
        {
            return new string(this.Select(x => x ? '1' : '0').ToArray());
        }

        public IEnumerator<bool> GetEnumerator()
        {
            for (var i = 0; i < Length; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static uint TakeBits(int count, uint value)
        {
            if (count is < 1 or > MaxLength) throw new ArgumentOutOfRangeException(nameof(count));
            return value & (uint)((1 << count) - 1);
        }

        public static BitArray operator &(BitArray a, BitArray b)
        {
            return new BitArray(a.Length, a.Value & b.Value);
        }

        public static BitArray operator |(BitArray a, BitArray b)
        {
            return new BitArray(a.Length, a.Value | b.Value);
        }

        public static BitArray operator ~(BitArray source)
        {
            return source.Complement;
        }

        public static implicit operator uint(BitArray source)
        {
            return source.Value;
        }
    }
}
