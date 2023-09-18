using Astrolo.Explorer.UI.Visualisation.Filtering;
using Astrolo.YiJing;
using Caliburn.Micro;

namespace Astrolo.Explorer.UI.Visualisation.Hive
{
    public sealed class HexChartViewModel : Screen, ISupportFiltering
    {
        public HexChartViewModel(ISequence sequence)
        {
            DisplayName = "Chart";

            const double radius = 42;

            LowerCells = sequence.GenerateLowerChart(radius).ToList();
            UpperCells = sequence.GenerateUpperChart(radius).ToList();
        }

        public IReadOnlyCollection<HexChartCell> UpperCells { get; }

        public IReadOnlyCollection<HexChartCell> LowerCells { get; }

        public void UpdateSelection(IHexagramFilter filter)
        {
            foreach (var cell in LowerCells.Concat(UpperCells))
            {
                cell.IsSelected = filter.Includes(cell.Hexagram);
            }
        }
    }
}
