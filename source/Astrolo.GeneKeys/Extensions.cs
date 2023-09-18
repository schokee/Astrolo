using Astrolo.Core;
using Astrolo.HumanDesign.Charting;
using Astrolo.YiJing;

namespace Astrolo.GeneKeys
{
    public static class Extensions
    {
        private static (Sphere Sphere, DesignElementAttribute Mapping)[] GeneKeyMap { get; } = ChartMetrics
            .EnumerateSpheres()
            .Select(sphere => (sphere, sphere.GetAttributesOfType<DesignElementAttribute>()!.First()))
            .ToArray();

        public static IEnumerable<T> SelectGeneKeys<T>(this PersonalChart chart, Func<Sphere, ChartValue, T> select)
        {
            return GeneKeyMap.Select(x => select(x.Sphere, chart.GetValue(x.Mapping.Marker, x.Mapping.InDesign)));
        }

        public static bool Includes(this Sequence sequence, Sphere sphere)
        {
            return sequence switch
            {
                Sequence.Activation => sphere is Sphere.LifesWork or Sphere.Evolution or Sphere.Radiance or Sphere.Purpose,
                Sequence.Venus => sphere is Sphere.Purpose or Sphere.Attraction or Sphere.IQ or Sphere.EQ or Sphere.SQ or Sphere.Vocation,
                Sequence.Pearl => sphere is Sphere.Vocation or Sphere.Culture or Sphere.Pearl or Sphere.LifesWork,
                Sequence.StarPearl => sphere is Sphere.Relating or Sphere.Stability or Sphere.Creativity,
                _ => false
            };
        }

        public static LineOfHexagram ToLineOfHexagram(this IGeneKey key, int line)
        {
            return new LineOfHexagram(key.Number, line);
        }
    }
}
