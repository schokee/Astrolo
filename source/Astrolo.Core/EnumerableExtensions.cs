using System;
using System.Collections.Generic;
using System.Linq;

namespace Astrolo.Core;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> o) where T : class
    {
        return o.Where(x => x is not null)!;
    }

    public static IReadOnlyDictionary<TKey, T> ToSortedList<TKey, T>(this IEnumerable<T> source, Func<T, TKey> getKey)
        where TKey : notnull
    {
        var result = new SortedList<TKey, T>();

        foreach (var item in source)
        {
            result.Add(getKey(item), item);
        }

        return result;
    }

    public static IReadOnlyDictionary<TKey, TValue> ToSortedList<T, TKey, TValue>(this IEnumerable<T> source, Func<T, TKey> getKey, Func<T, TValue> getValue)
        where TKey : notnull
    {
        var result = new SortedList<TKey, TValue>();

        foreach (var item in source)
        {
            result.Add(getKey(item), getValue(item));
        }

        return result;
    }
}
