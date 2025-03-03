using System.Collections.Generic;
using System.Linq;

namespace Astrolo.Astrology;

public static class Zodiac
{
    private static SortedList<ZodiacSign, ZodiacSignInfo> Lookup { get; }

    static Zodiac()
    {
        Lookup = new SortedList<ZodiacSign, ZodiacSignInfo>();

        foreach (var info in ZodiacSignInfo.GenerateSigns())
        {
            Lookup[info.Sign] = info;
        }
    }

    public static IEnumerable<ZodiacSignInfo> AllSigns => Lookup.Values;

    public static ZodiacSignInfo SignAt(int day, int month)
    {
        return AllSigns.First(x => x.Contains(day, month));
    }

    public static ZodiacSignInfo Info(this ZodiacSign sign) => Lookup[sign];
}
