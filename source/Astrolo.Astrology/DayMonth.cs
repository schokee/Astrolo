using System;

namespace Astrolo.Astrology;

public readonly struct DayMonth(int day, int month) : IComparable<DayMonth>, IComparable
{
    public static DayMonth MinimumValue { get; } = new(1, 1);

    public static DayMonth MaximumValue { get; } = new(31, 12);

    private readonly DateTime _date = new(2000, month, day);

    private DayMonth(DateTime dateTime) : this(dateTime.Day, dateTime.Month)
    {
    }

    public int Month => _date.Month;

    public int Day => _date.Day;

    public DayMonth AddDays(int days)
    {
        return new(_date.AddDays(days));
    }

    public override string ToString()
    {
        return _date.ToString("MMM d");
    }

    public int CompareTo(DayMonth other)
    {
        return _date.CompareTo(other._date);
    }

    public int CompareTo(object obj)
    {
        return ReferenceEquals(null, obj)
            ? 1
            : obj is DayMonth other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(DayMonth)}");
    }

    public static bool operator <(DayMonth left, DayMonth right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(DayMonth left, DayMonth right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(DayMonth left, DayMonth right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(DayMonth left, DayMonth right)
    {
        return left.CompareTo(right) >= 0;
    }
}
