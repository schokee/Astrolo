using Astrolo.YiJing;

namespace Astrolo.HumanDesign;

public static class Extensions
{
    public static LineOfHexagram ToLineOfHexagram(this ILineInfo line)
    {
        return new(line.Gate.Number, line.Number);
    }

    public static string Channels(this IGateInfo gate)
    {
        return string.Join(", ", gate.Channels.Select(x => x.Name));
    }

    public static bool HasCompleteChannel(this IGateInfo gate, Func<IGateInfo, bool> isActive)
    {
        return gate.Channels.Any(x => isActive(x.StartGate) && isActive(x.EndGate));
    }

    public static IEnumerable<T> SelectHarmonics<T>(this IGateInfo gate, Func<IGateInfo, T> getEnd)
    {
        return gate.Channels.Select(channel =>
            getEnd(ReferenceEquals(channel.StartGate, gate) ? channel.EndGate : channel.StartGate));
    }

    public static IEnumerable<IChannel> SelectChannels(this IGateDictionary gateDictionary)
    {
        return gateDictionary.Values.SelectMany(x => x.Channels);
    }
}
