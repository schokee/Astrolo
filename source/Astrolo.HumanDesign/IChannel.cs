namespace Astrolo.HumanDesign;

public interface IChannel : IEnumerable<IGateInfo>
{
    string Name { get; }

    IGateInfo StartGate { get; }

    IGateInfo EndGate { get; }
}
