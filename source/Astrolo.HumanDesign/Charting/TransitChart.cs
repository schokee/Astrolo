using Astrolo.YiJing;

namespace Astrolo.HumanDesign.Charting
{
    public sealed class TransitChart : Chart
    {
        public TransitChart(IGateDictionary gateLookup, LineOfHexagram lineOfHexagram, Func<IGateInfo, bool> isGateActive)
            : base(gateLookup, gate => new GateConfiguration(gate, isGateActive(gate)))
        {
            LineOfHexagram = lineOfHexagram;
        }

        public LineOfHexagram LineOfHexagram { get; }

        private class GateConfiguration : IGateConfiguration
        {
            public GateConfiguration(IGateInfo gate, bool isActive)
            {
                Gate = gate;
                IsActive = isActive;
            }

            public IGateInfo Gate { get; }

            public bool IsActive { get; }

            public GateActivation ActivationState => IsActive ? GateActivation.Personality : GateActivation.None;
        }
    }
}
