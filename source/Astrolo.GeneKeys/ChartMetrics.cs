using Astrolo.Geometry;

namespace Astrolo.GeneKeys;

public sealed class ChartMetrics
{
    public ChartMetrics(Rectangle bounds) : this(bounds.SmallestSize() / 9, bounds)
    {
    }

    public ChartMetrics(double sphereSize, Rectangle bounds)
    {
        var size = Math.Max(9 * sphereSize, bounds.SmallestSize());
        var center = bounds.Center;
        var radius = sphereSize + (size - 5 * sphereSize) / 4;

        SphereSize = sphereSize;
        InnerCircle = new Circle(center, radius);
        OuterCircle = new Circle(center, radius * 2);
        PearlCircle = new Circle(InnerCircle.PointAt(Angle.FromDegrees(0)), radius);
        Bounds = new Circle(center, size / 2).Bounds;
    }

    public double SphereSize { get; }

    public Circle InnerCircle { get; }

    public Circle OuterCircle { get; }

    public Circle PearlCircle { get; }

    public Rectangle Bounds { get; }

    public Circle CircleOf(Sphere sphere)
    {
        return new Circle(CenterOf(sphere), SphereSize / 2);
    }

    public Point CenterOf(Sphere sphere)
    {
        return sphere switch
        {
            Sphere.LifesWork => OuterCircle.PointAt(Angle.FromDegrees(0)),
            Sphere.Evolution => OuterCircle.PointAt(Angle.FromDegrees(90)),
            Sphere.Radiance => OuterCircle.PointAt(Angle.FromDegrees(270)),
            Sphere.Purpose => OuterCircle.PointAt(Angle.FromDegrees(180)),
            Sphere.Attraction => InnerCircle.PointAt(Angle.FromDegrees(180)),
            Sphere.IQ => InnerCircle.PointAt(Angle.FromDegrees(240)),
            Sphere.EQ => InnerCircle.PointAt(Angle.FromDegrees(120)),
            Sphere.SQ => InnerCircle.Center,
            Sphere.Vocation => InnerCircle.PointAt(Angle.FromDegrees(300)),
            Sphere.Culture => InnerCircle.PointAt(Angle.FromDegrees(60)),
            Sphere.Pearl => InnerCircle.PointAt(Angle.FromDegrees(0)),
            Sphere.Stability => PearlCircle.PointAt(Angle.FromDegrees(180)),
            Sphere.Creativity => PearlCircle.PointAt(Angle.FromDegrees(300)),
            Sphere.Relating => PearlCircle.PointAt(Angle.FromDegrees(60)),
            _ => throw new ArgumentOutOfRangeException(nameof(sphere), sphere, null)
        };
    }

    public IEnumerable<Point> PointsInPathway(Pathway pathway)
    {
        return TraversePathway(pathway).Select(CenterOf);
    }

    public IEnumerable<Point> PointsInSequence(Sequence sequence)
    {
        return TraverseSequence(sequence).Select(CenterOf);
    }

    public static IEnumerable<Sphere> EnumerateSpheres()
    {
        return Enum.GetValues(typeof(Sphere)).Cast<Sphere>();
    }

    public static IEnumerable<Sphere> EnumeratePathPoints()
    {
        yield return Sphere.LifesWork;
        yield return Sphere.Evolution;
        yield return Sphere.Radiance;
        yield return Sphere.Purpose;
        yield return Sphere.Attraction;
        yield return Sphere.IQ;
        yield return Sphere.EQ;
        yield return Sphere.SQ;
        yield return Sphere.Vocation;
        yield return Sphere.Culture;
        yield return Sphere.Pearl;
    }

    private static IEnumerable<Sphere> TraversePathway(Pathway pathway)
    {
        return pathway switch
        {
            Pathway.Challenge => [Sphere.LifesWork, Sphere.Evolution],
            Pathway.Breakthrough => [Sphere.Evolution, Sphere.Radiance],
            Pathway.CoreStability => [Sphere.Radiance, Sphere.Purpose],
            Pathway.Dharma => [Sphere.Purpose, Sphere.Attraction],
            Pathway.Karma => [Sphere.Attraction, Sphere.IQ],
            Pathway.Intelligence => [Sphere.IQ, Sphere.EQ],
            Pathway.Love => [Sphere.EQ, Sphere.SQ],
            Pathway.Realisation => [Sphere.SQ, Sphere.Vocation],
            Pathway.Initiative => [Sphere.Vocation, Sphere.Culture],
            Pathway.Growth => [Sphere.Culture, Sphere.LifesWork],
            Pathway.Service => [Sphere.LifesWork, Sphere.Vocation],
            Pathway.Quantum => new[] { Sphere.LifesWork, Sphere.Pearl, Sphere.Vocation, Sphere.Pearl, Sphere.Culture },
            _ => throw new ArgumentOutOfRangeException(nameof(pathway), pathway, null)
        };
    }

    private static IEnumerable<Sphere> TraverseSequence(Sequence sequence)
    {
        switch (sequence)
        {
            case Sequence.Activation:
                yield return Sphere.LifesWork;
                yield return Sphere.Evolution;
                yield return Sphere.Radiance;
                yield return Sphere.Purpose;
                break;
            case Sequence.Venus:
                yield return Sphere.Purpose;
                yield return Sphere.Attraction;
                yield return Sphere.IQ;
                yield return Sphere.EQ;
                yield return Sphere.Vocation;
                break;
            case Sequence.Pearl:
                yield return Sphere.Vocation;
                yield return Sphere.Culture;
                yield return Sphere.Pearl;
                yield return Sphere.Vocation;
                yield return Sphere.LifesWork;
                yield return Sphere.Culture;
                yield return Sphere.Pearl;
                yield return Sphere.LifesWork;
                break;
            case Sequence.StarPearl:
                yield return Sphere.Stability;
                yield return Sphere.Relating;
                yield return Sphere.Pearl;
                yield return Sphere.Relating;
                yield return Sphere.Creativity;
                yield return Sphere.Stability;
                yield return Sphere.Pearl;
                yield return Sphere.Creativity;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sequence), sequence, null);
        }
    }

}