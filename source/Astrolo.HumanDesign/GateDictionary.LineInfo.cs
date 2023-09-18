using Astrolo.YiJing;

namespace Astrolo.HumanDesign
{
    public sealed partial class GateDictionary
    {
        private sealed class LineInfo : ILineInfo
        {
            public LineInfo(IGateInfo gate, Serialization.LineRecord record)
            {
                if (record.Line is < 1 or > HexagramInfo.TotalLines) throw new ArgumentOutOfRangeException(nameof(record.Line));

                Gate = gate;
                Number = record.Line;
                Theme = record.Theme!;
                MandalaSlice = record.ToLocation();
            }

            public IGateInfo Gate { get; }
            public int Number { get; }
            public MandalaSlice MandalaSlice { get; }
            public string Theme { get; }

            public override string ToString()
            {
                return $"{Gate.Number}.{Number}";
            }
        }
    }
}
