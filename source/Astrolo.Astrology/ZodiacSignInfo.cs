using System.Collections.Generic;
using System.Linq;

namespace Astrolo.Astrology
{
    public sealed class ZodiacSignInfo
    {
        private ZodiacSignInfo(ZodiacSign sign, Period period, Element element, Modality modality, Marker ruler, House house)
        {
            Period = period;
            Sign = sign;
            Element = element;
            Modality = modality;
            Ruler = ruler;
            House = house;
            SignWithPeriod = $"{Sign} ({StartDay} - {EndDay})";
        }

        public ZodiacSign Sign { get; }

        public House House { get; }

        public Element Element { get; }

        public Marker Ruler { get; }

        public Modality Modality { get; }

        public Polarity Polarity => (Polarity)((int)Sign % 2);

        public DayMonth StartDay => Period.Start;

        public DayMonth EndDay => Period.End.AddDays(-1);   // Inclusive end

        public string SignWithPeriod { get; }

        public bool Contains(int day, int month)
        {
            return Period.Contains(new DayMonth(day, month));
        }

        public override string ToString()
        {
            return SignWithPeriod;
        }

        public static implicit operator ZodiacSignInfo(ZodiacSign sign)
        {
            return sign.Info();
        }

        internal Period Period { get; }

        internal static IEnumerable<ZodiacSignInfo> GenerateSigns()
        {
            // See also: https://en.wikipedia.org/wiki/Western_astrology

            var startDates = new DayMonth[]
            {
                new(20, 1),    // Aquarius: Jan 20
                new(19, 2),
                new(21, 3),
                new(20, 4),
                new(21, 5),
                new(22, 6),
                new(23, 7),
                new(23, 8),
                new(23, 9),
                new(23, 10),
                new(22, 11),
                new(22, 12)    // Capricorn: Dec 22
            };

            var endDates = startDates
                .Skip(1)
                .Concat(startDates);

            var period = startDates
                .Zip(endDates, (start, end) => new Period(start, end))
                .ToList();

            yield return new(ZodiacSign.Aquarius, period[0],
                Element.Air, Modality.Fixed, Marker.Uranus, House.Blessings);

            yield return new(ZodiacSign.Pisces, period[1],
                Element.Water, Modality.Mutable, Marker.Neptune, House.Sacrifice);

            yield return new(ZodiacSign.Aries, period[2],
                Element.Fire, Modality.Cardinal, Marker.Mars, House.Self);

            yield return new(ZodiacSign.Taurus, period[3],
                Element.Earth, Modality.Fixed, Marker.Venus, House.Value);

            yield return new(ZodiacSign.Gemini, period[4],
                Element.Air, Modality.Mutable, Marker.Mercury, House.Sharing);

            yield return new(ZodiacSign.Cancer, period[5],
                Element.Water, Modality.Cardinal, Marker.Moon, House.HomeAndFamily);

            yield return new(ZodiacSign.Leo, period[6],
                Element.Fire, Modality.Fixed, Marker.Sun, House.Pleasure);

            yield return new(ZodiacSign.Virgo, period[7],
                Element.Earth, Modality.Mutable, Marker.Mercury, House.Health);

            yield return new(ZodiacSign.Libra, period[8],
                Element.Air, Modality.Cardinal, Marker.Venus, House.Balance);

            yield return new(ZodiacSign.Scorpio, period[9],
                Element.Water, Modality.Fixed, Marker.Pluto, House.Transformation);

            yield return new(ZodiacSign.Sagittarius, period[10],
                Element.Fire, Modality.Mutable, Marker.Jupiter, House.Purpose);

            yield return new(ZodiacSign.Capricorn, period[11],
                Element.Earth, Modality.Cardinal, Marker.Saturn, House.Enterprise);
        }
    }
}
