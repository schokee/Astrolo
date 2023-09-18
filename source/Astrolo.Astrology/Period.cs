namespace Astrolo.Astrology
{
    internal readonly struct Period
    {
        public Period(DayMonth start, DayMonth end)
        {
            Start = start;
            End = end;
        }

        public DayMonth Start { get; }

        public DayMonth End { get; }

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
}
