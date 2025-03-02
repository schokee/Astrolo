using Astrolo.Core;

namespace Astrolo.HumanDesign;

public sealed class CenterMap : IEnumerable<KeyValuePair<Center, CenterState>>, IEquatable<CenterMap>
{
    private static (Center Center, Authority Authority)[] AuthorityMapping { get; } =
    [
        ( Center.SolarPlexus, Authority.Emotional ),
        ( Center.Sacral, Authority.Sacral ),
        ( Center.Spleen, Authority.Splenic ),
        ( Center.Heart, Authority.Ego ),
        ( Center.G, Authority.G ),
        ( Center.Ajna, Authority.Mental ),
        ( Center.Head, Authority.Mental )
    ];

    public static IReadOnlyCollection<Center> AllCenters { get; } = Enum.GetValues(typeof(Center)).Cast<Center>().ToList();

    private Dictionary<Center, CenterState> Lookup { get; } = new();

    public CenterMap()
    {
        foreach (var center in AllCenters)
        {
            Lookup.Add(center, CenterState.Undefined);
        }
    }

    public CenterMap(IEnumerable<KeyValuePair<Center, CenterState>> other) : this()
    {
        foreach (var pair in other ?? throw new ArgumentNullException(nameof(other)))
        {
            Lookup[pair.Key] = pair.Value;
        }
    }

    public CenterState this[Center center] => Lookup[center];

    public IEnumerable<Center> Defined => Lookup.Where(x => x.Value == CenterState.Defined).Select(x => x.Key);

    public bool IsDefined(Center center) => this[center] == CenterState.Defined;

    public int Value => Lookup.Select(x => (int)x.Value).Aggregate(0, (result, x) => result * 3 + x);

    public bool AllOpen => !AnyDefined;

    public bool AnyDefined => Lookup.Values.Any(x => x == CenterState.Defined);

    public int TotalDefined => Lookup.Values.Count(x => x == CenterState.Defined);

    public Authority Authority => AnyDefined ? AuthorityMapping.First(x => IsDefined(x.Center)).Authority : Authority.Lunar;

    //public CenterMap IntersectWith(CenterMap other)
    //{
    //    if (other == null) throw new ArgumentNullException(nameof(other));
    //    return new CenterMap(this.Zip(other, (a, b) => new KeyValuePair<Center, bool>(a.Key, a.Value && b.Value)));
    //}

    public IEnumerator<KeyValuePair<Center, CenterState>> GetEnumerator()
    {
        return Lookup.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Equals(CenterMap? other)
    {
        return ReferenceEquals(this, other) || Value == other?.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CenterMap);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return AllOpen ? "All Open" : string.Join("/", AllCenters.Where(IsDefined).Select(x => x.GetDescriptionOrDefault()));
    }

    public static bool operator ==(CenterMap left, CenterMap right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CenterMap left, CenterMap right)
    {
        return !Equals(left, right);
    }
}
