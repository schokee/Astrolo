using Astrolo.Astrology;
using NUnit.Framework;

namespace Astrolo.Tests.Astrology
{
    public class ZodiacSignsFixture
    {
        [TestCase(1, 1, ExpectedResult = ZodiacSign.Capricorn)]
        [TestCase(8, 2, ExpectedResult = ZodiacSign.Aquarius)]
        [TestCase(19, 2, ExpectedResult = ZodiacSign.Pisces)]
        [TestCase(2, 12, ExpectedResult = ZodiacSign.Sagittarius)]
        [TestCase(26, 12, ExpectedResult = ZodiacSign.Capricorn)]
        public ZodiacSign TestSignAt(int day, int month)
        {
            return Zodiac.SignAt(day, month).Sign;
        }

        [Test]
        public void DumpAll()
        {
            foreach (ZodiacSignInfo info in Zodiac.AllSigns)
            {
                TestContext.WriteLine(info);
            }
        }
    }
}
