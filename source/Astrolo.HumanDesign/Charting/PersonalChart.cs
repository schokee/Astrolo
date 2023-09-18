using Astrolo.Astrology;
using Astrolo.YiJing;

namespace Astrolo.HumanDesign.Charting
{
    public sealed class PersonalChart : Chart
    {
        private static IReadOnlyCollection<IncarnationCross> IncarnationCrosses { get; } = IncarnationCross.Load().ToList();

        private PersonalChart(IGateDictionary gateDictionary, Func<IGateInfo, IGateConfiguration> getStateOfGate,
            ValueArray designValues,
            ValueArray personalityValues) : base(gateDictionary, getStateOfGate)
        {
            Design = designValues;
            Personality = personalityValues;
            Type = $"{GetValue(Marker.Sun, false).Line.Number}/{GetValue(Marker.Sun, true).Line.Number}";

            var crossGates = IncarnationCrossValues.Take(4).Select(x => x.LineOfHexagram).ToArray();
            IncarnationCross = IncarnationCrosses.Single(x => x.Matches(crossGates));
        }

        public string Type { get; }

        public IncarnationCross IncarnationCross { get; }

        public IEnumerable<ChartValue> Values => Design.Concat(Personality);

        public ValueArray Design { get; }

        public ValueArray Personality { get; }

        public ChartValue GetValue(Marker figure, bool inDesign) => (inDesign ? Design : Personality)[figure];

        public IEnumerable<ChartValue> IncarnationCrossValues
        {
            get
            {
                yield return GetValue(Marker.Sun, false);
                yield return GetValue(Marker.Earth, false);
                yield return GetValue(Marker.Sun, true);
                yield return GetValue(Marker.Earth, true);
            }
        }

        public PersonalChart Filter(bool designOnly)
        {
            return new PersonalChart(GateDictionary, gate => ((GateConfiguration)this[gate]).Filter(designOnly), Design, Personality);
        }

        public static bool IsValidIncarnationCross(LineOfHexagram[] values)
        {
            return values?.Length == 4 && IncarnationCrosses.Any(x => x.Matches(values));
        }

        public static PersonalChart Create(IGateDictionary gateLookup, Func<Marker, bool, LineOfHexagram> getValue)
        {
            ChartValue CreateChartValue(Marker figure, bool isDesign)
            {
                var pair = getValue(figure, isDesign);
                return new ChartValue(figure, isDesign, gateLookup[pair]);
            }

            var designValues = ValueArray.Create(e => CreateChartValue(e, true));
            var personalityValues = ValueArray.Create(e => CreateChartValue(e, false));
            var activatedGates = designValues.Concat(personalityValues);

            var states = gateLookup.Values
                .GroupJoin(activatedGates, x => x.Number, x => x.Gate.Number, (gate, values) => (IGateConfiguration)new GateConfiguration(gate, values.ToLookup(x => x.IsDesign, x => x.Marker)))
                .ToDictionary(x => x.Gate.Number);

            return new PersonalChart(gateLookup, gate => states[gate.Number], designValues, personalityValues);
        }

        public class ValueArray : IEnumerable<ChartValue>
        {
            public static IReadOnlyList<Marker> Elements { get; } = Enum
                .GetValues(typeof(Marker))
                .Cast<Marker>()
                .OrderBy(ToOrdinal)
                .ToList();

            private IReadOnlyList<ChartValue> Values { get; }

            private ValueArray(IReadOnlyList<ChartValue> values)
            {
                Values = values;
            }

            public ChartValue this[Marker figure] => Values[ToOrdinal(figure)];

            public IEnumerator<ChartValue> GetEnumerator()
            {
                return Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public LineOfHexagram[] ToArray()
            {
                return Values.Select(x => x.LineOfHexagram).ToArray();
            }

            public static ValueArray Create(Func<Marker, ChartValue> getValue)
            {
                return new ValueArray(Elements
                    .Select(element => getValue(element) ?? throw new NullReferenceException())
                    .ToArray());
            }

            private static int ToOrdinal(Marker figure)
            {
                switch (figure)
                {
                    case Marker.Sun:
                        return 0;
                    case Marker.Earth:
                        return 1;
                    case Marker.NorthNode:
                        return 2;
                    case Marker.SouthNode:
                        return 3;
                    case Marker.Moon:
                        return 4;
                    case Marker.Mercury:
                        return 5;
                    case Marker.Venus:
                        return 6;
                    case Marker.Mars:
                    case Marker.Jupiter:
                    case Marker.Saturn:
                    case Marker.Uranus:
                    case Marker.Neptune:
                    case Marker.Pluto:
                        return (int)figure + 2;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(figure), figure, null);
                }
            }
        }

        private class GateConfiguration : IGateConfiguration
        {
            public GateConfiguration(IGateInfo gate, ILookup<bool, Marker> values)
            {
                Gate = gate;
                ActiveMarkersInDesign = values[true].ToList();
                ActiveMarkersInPersonality = values[false].ToList();

                if (ActiveMarkersInDesign.Any())
                    ActivationState |= GateActivation.Design;

                if (ActiveMarkersInPersonality.Any())
                    ActivationState |= GateActivation.Personality;
            }

            public IGateInfo Gate { get; }

            public bool IsActive => ActivationState != GateActivation.None;
            public GateActivation ActivationState { get; }

            public IReadOnlyList<Marker> ActiveMarkersInDesign { get; }
            public IReadOnlyList<Marker> ActiveMarkersInPersonality { get; }

            public GateConfiguration Filter(bool designOnly)
            {
                var subset = designOnly ? ActiveMarkersInDesign : ActiveMarkersInPersonality;
                return new GateConfiguration(Gate, subset.ToLookup(_ => designOnly));
            }

            public override string ToString()
            {
                return $"{Gate.Number} (IsActive={IsActive})";
            }
        }
    }
}
