namespace Astrolo.GeneKeys;

public interface ICodonRing : IFormattable
{
    CodonRing Id { get; }

    string Name { get; }

    IReadOnlyList<int> Sequence { get; }

    string ArcanaArchetype { get; }
}