using Astrolo.Core;

namespace Astrolo.HumanDesign.Charting;

public class RelationshipChart : Chart
{
    private RelationshipChart(IGateDictionary gateDictionary, Func<IGateInfo, IGateConfiguration?> getStateOfGate, IReadOnlyList<ChannelComparison> channels)
        : base(gateDictionary, getStateOfGate)
    {
        Channels = channels;
    }

    public IReadOnlyList<ChannelComparison> Channels { get; }

    public static RelationshipChart Create(IGateDictionary gateDictionary, PersonalChart lhs, PersonalChart rhs, ChannelMatch matchType = ChannelMatch.Dominant)
    {
        var channels = gateDictionary
            .SelectChannels()
            .Distinct()
            .Select(channel => ChannelComparison.Evaluate(channel, lhs, rhs))
            .WhereNotNull()
            .OrderBy(x => x.Assessment)
            .ThenBy(x => x.Activation)
            .ToList();

        var lookup = channels
            .Where(x => x.Assessment == matchType)
            .SelectMany(x => x.Gates)
            .GroupBy(x => x.Gate.Number)
            .ToLookup(x => x.Key, x => x.Aggregate(default(ChannelComparison.ActiveGate), (result, gate) => result?.CombineWith(gate) ?? gate));

        return new(gateDictionary, gate => lookup[gate.Number].FirstOrDefault(), channels);
    }
}
