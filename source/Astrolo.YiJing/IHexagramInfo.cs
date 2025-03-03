namespace Astrolo.YiJing;

public interface IHexagramInfo
{
    public int Number { get; }

    public string Title { get; }

    public Trigram LowerTrigram { get; }

    public Trigram UpperTrigram { get; }

    public YiSpherePoint SphereLocation { get; }
}
