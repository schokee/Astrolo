using Astrolo.Core;

namespace Astrolo.HumanDesign.Charting;
// conditioning receptor = defined gate in an undefined center
// mental conditioner    = undefined gate in an undefined center

public abstract class Chart
{
    protected IGateDictionary GateDictionary { get; }
    protected IReadOnlyDictionary<int, IGateConfiguration> Gates { get; }

    protected Chart(IGateDictionary gateDictionary, Func<IGateInfo, IGateConfiguration?> getStateOfGate) : this(gateDictionary, getStateOfGate, x => x.IsActive)
    {
    }

    protected Chart(IGateDictionary gateDictionary, Func<IGateInfo, IGateConfiguration?> getStateOfGate, Func<IGateConfiguration, bool> isActive)
    {
        GateDictionary = gateDictionary;

        var allGates = gateDictionary.Values.ToSortedList(x => x.Number, x => getStateOfGate(x) ?? new InactiveGate(x));
        var graphs = CountSubgraphs(allGates);

        Splits = graphs > 1 ? graphs : 0;
        Gates = allGates;
        Centers = new CenterMap(allGates.Values
            .Select(x => x.Gate)
            .GroupBy(x => x.Center)
            .ToDictionary(x => x.Key, x => GetState(x.ToList(), gate => isActive(allGates[gate.Number]))));
    }

    public IGateConfiguration this[int gate] => Gates[gate];

    public IGateConfiguration this[IGateInfo gate] => Gates[gate.Number];

    public CenterMap Centers { get; }

    public bool IsSplitDefinition => Splits > 0;

    public int Splits { get; }

    public string Definition
    {
        get
        {
            return Centers.AllOpen
                ? "None"
                : Splits switch
                {
                    0 => "Single",
                    2 => "Split",
                    3 => "Triple Split",
                    4 => "Quadruple Split",
                    _ => "Unknown"
                };
        }
    }

    public IEnumerable<IChannel> CompleteChannels => Gates
        .Values
        .AsParallel()
        .Where(x => x.IsActive)
        .SelectMany(x => x.Gate.Channels)
        .Where(IsChannelComplete)
        .Distinct()
        .OrderBy(x => x.StartGate.Center)
        .ThenBy(x => x.StartGate.Number);

    public IEnumerable<IGateConfiguration> SelectHangingGates()
    {
        return Gates.Values
            .AsParallel()
            .Where(x => x.IsActive && !x.Gate.HasCompleteChannel(gate => this[gate.Number].IsActive));
    }

    public IEnumerable<IGateConfiguration> SelectGatesCompletedBy(PersonalChart other)
    {
        return SelectHangingGates().Where(x => x.Gate.HasCompleteChannel(gate => other[gate.Number].IsActive));
    }

    protected bool IsChannelUndefined(IChannel channel)
    {
        return !this[channel.StartGate].IsActive && !this[channel.EndGate].IsActive;
    }

    protected bool IsChannelComplete(IChannel channel)
    {
        return this[channel.StartGate].IsActive && this[channel.EndGate].IsActive;
    }

    protected bool IsChannelPartiallyComplete(IChannel channel)
    {
        return this[channel.StartGate].IsActive || this[channel.EndGate].IsActive;
    }

    private static CenterState GetState(IReadOnlyList<IGateInfo> allGatesInCenter, Func<IGateInfo, bool> isGateActive)
    {
        var result = allGatesInCenter.Any(isGateActive)
            ? allGatesInCenter.Any(g => g.HasCompleteChannel(isGateActive)) ? CenterState.Defined : CenterState.Undefined
            : CenterState.Open;

        return result;
    }

    private static int CountSubgraphs(IReadOnlyDictionary<int, IGateConfiguration> allGates)
    {
        var activatedCenters = allGates.Values
            .Where(x => x.IsActive)
            .Select(x => x.Gate)
            .Where(x => x.HasCompleteChannel(g => allGates[g.Number].IsActive))
            .GroupBy(x => x.Center, x => x.SelectHarmonics(g => g.Center))
            .ToDictionary(x => x.Key, x => x.SelectMany(l => l).Distinct());

        var total = 0;

        while (activatedCenters.Count > 0)
        {
            ++total;

            var toDo = new HashSet<Center> { activatedCenters.First().Key };

            while (toDo.Count > 0)
            {
                var center = toDo.First();
                var adjacent = activatedCenters[center];

                toDo.Remove(center);
                activatedCenters.Remove(center);

                foreach (var adjacentCenter in adjacent.Intersect(activatedCenters.Keys))
                {
                    toDo.Add(adjacentCenter);
                }
            }
        }

        return total;
    }

    protected class InactiveGate(IGateInfo gate) : IGateConfiguration
    {
        public IGateInfo Gate { get; } = gate;

        public bool IsActive => false;
        public GateActivation ActivationState => GateActivation.None;
    }
}
