namespace Astrolo.Astrology;

internal readonly struct Period(DayMonth start, DayMonth end)
{
    public DayMonth Start { get; } = start;

    public DayMonth End { get; } = end;

    public bool SpansStartOfYear => Start > End;

    public bool Contains(DayMonth time)
    {
        return SpansStartOfYear
            ? time >= Start || time < End
            : time >= Start && time < End;
    }

    public override string ToString()
    {
        return $"{Start} - {End}";
    }
}
