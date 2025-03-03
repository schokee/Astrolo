namespace Astrolo.YiJing;

public interface ISequence
{
    string Name { get; }

    IReadOnlyList<TrigramFigure> Trigrams { get; }

    IReadOnlyList<HexagramFigure> Hexagrams { get; }

    HexagramFigure[][] ToTable();
}
