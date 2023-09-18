using System.Collections.Generic;
using System.Linq;

namespace Astrolo.Core
{
    public static class ArrayEqualityComparer
    {
        public static IEqualityComparer<IReadOnlyCollection<T>> Create<T>(IEqualityComparer<T> comparer)
        {
            return new ArrayEqualityComparer<T>(comparer);
        }
    }

    public sealed class ArrayEqualityComparer<T> : IEqualityComparer<IReadOnlyCollection<T>>
    {
        public static IEqualityComparer<IReadOnlyCollection<T>> Default { get; } = new ArrayEqualityComparer<T>();

        private readonly IEqualityComparer<T> _elementComparer;

        public ArrayEqualityComparer() : this(EqualityComparer<T>.Default)
        {
        }

        public ArrayEqualityComparer(IEqualityComparer<T> elementComparer)
        {
            _elementComparer = elementComparer;
        }

        public bool Equals(IReadOnlyCollection<T>? x, IReadOnlyCollection<T>? y)
        {
            return ReferenceEquals(x, y) || x is not null && y is not null && x.Count == y.Count && x.Zip(y, _elementComparer.Equals).All(b => b);
        }

        public int GetHashCode(IReadOnlyCollection<T> array)
        {
            return array
                .Where(x => x is not null)
                .Aggregate(23, (current, item) => current * 31 + _elementComparer.GetHashCode(item!));
        }
    }
}
