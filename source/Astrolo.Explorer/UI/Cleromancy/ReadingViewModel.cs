using Astrolo.YiJing;
using Caliburn.Micro;

namespace Astrolo.Explorer.UI.Cleromancy;

public sealed class ReadingViewModel : Screen
{
    private Random Random { get; } = new();
    private Func<EditorViewModel> CreateEditor { get; }

    public ReadingViewModel(IEnumerable<IChangeGenerator> generators, ISequence sequence, Func<IEnumerable<Movement>, EditorViewModel> createEditor)
    {
        DisplayName = "I Ching";

        CreateEditor = () => createEditor(Enumerable
            .Range(0, HexagramInfo.TotalLines)
            .Select(_ => SelectedGenerator.Generate(Random.Next)));

        Hexagrams = sequence.Hexagrams.OrderBy(x => x.Number).ToList();
        Generators = generators.ToList();
        SelectedGenerator = Generators.FirstOrDefault();

        Generate();
    }

    public IReadOnlyList<HexagramFigure> Hexagrams { get; }

    public IReadOnlyList<IChangeGenerator> Generators { get; }

    public IChangeGenerator SelectedGenerator
    {
        get;
        set => Set(ref field, value);
    }

    public EditorViewModel Editor
    {
        get;
        private set => Set(ref field, value);
    }

    public void Generate()
    {
        Editor = CreateEditor();
    }

    public bool IsSelectable { get; set; }
    public bool IsSelected { get; set; }
}
