using System.Collections.Generic;
using System.Linq;

namespace Astrolo.Core;

public static class ArrayEqualityComparer
{
    public static IEqualityComparer<IReadOnlyCollection<T>> Create<T>(IEqualityComparer<T> comparer)
    {
        return new ArrayEqualityComparer<T>(comparer);
    }
}

public sealed class ArrayEqualityComparer<T>(IEqualityComparer<T> elementComparer) : IEqualityComparer<IReadOnlyCollection<T>>
{
    public static IEqualityComparer<IReadOnlyCollection<T>> Default { get; } = new ArrayEqualityComparer<T>();

    public ArrayEqualityComparer() : this(EqualityComparer<T>.Default)
    {
    }

    public bool Equals(IReadOnlyCollection<T>? x, IReadOnlyCollection<T>? y)
    {
        return ReferenceEquals(x, y) || x is not null && y is not null && x.Count == y.Count && x.Zip(y, elementComparer.Equals).All(b => b);
    }

    public int GetHashCode(IReadOnlyCollection<T> array)
    {
        return array
            .Where(x => x is not null)
            .Aggregate(23, (current, item) => current * 31 + elementComparer.GetHashCode(item!));
    }
}
