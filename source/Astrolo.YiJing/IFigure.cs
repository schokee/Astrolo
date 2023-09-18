namespace Astrolo.YiJing
{
    public interface IFigure : IEnumerable<Line>
    {
        int LineCount { get; }
    }
}
