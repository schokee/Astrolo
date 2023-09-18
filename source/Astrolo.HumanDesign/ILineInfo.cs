namespace Astrolo.HumanDesign
{
    public interface ILineInfo
    {
        IGateInfo Gate { get; }

        int Number { get; }

        MandalaSlice MandalaSlice { get; }

        string Theme { get; }
    }
}
