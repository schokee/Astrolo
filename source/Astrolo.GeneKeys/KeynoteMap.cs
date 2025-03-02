using Astrolo.GeneKeys.Metadata;

namespace Astrolo.GeneKeys;

public sealed class KeynoteMap
{
    private IReadOnlyDictionary<(Sphere, int), string> Keynotes { get; }

    public KeynoteMap()
    {
        Keynotes = EmbeddedFile.DeserializeCsv<Record>("Keynotes.csv").ToDictionary(x => (x.Sphere, x.Line), x => x.Keynote!);
    }

    public string this[Sphere sphere, int line] => Keynotes.TryGetValue((sphere, line), out var note) ? note : string.Empty;

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Record
    {
        public Sphere Sphere { get; set; }
        public int Line { get; set; }
        public string? Keynote { get; set; }
    }
}