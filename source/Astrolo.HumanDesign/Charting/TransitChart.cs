using Astrolo.YiJing;

namespace Astrolo.HumanDesign.Charting;

public sealed class TransitChart(
    IGateDictionary gateLookup,
    LineOfHexagram lineOfHexagram,
    Func<IGateInfo, bool> isGateActive)
    : Chart(gateLookup, gate => new GateConfiguration(gate, isGateActive(gate)))
{
    public LineOfHexagram LineOfHexagram { get; } = lineOfHexagram;

    private class GateConfiguration(IGateInfo gate, bool isActive) : IGateConfiguration
    {
        public IGateInfo Gate { get; } = gate;

        public bool IsActive { get; } = isActive;

        public GateActivation ActivationState => IsActive ? GateActivation.Personality : GateActivation.None;
    }
}
