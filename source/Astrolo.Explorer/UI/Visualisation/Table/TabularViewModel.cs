using Astrolo.Explorer.UI.Visualisation.Filtering;
using Astrolo.GeneKeys;
using Astrolo.YiJing;
using Caliburn.Micro;

namespace Astrolo.Explorer.UI.Visualisation.Table
{
    public sealed class TabularViewModel : Screen, ISupportFiltering
    {
        public TabularViewModel(ISequence sequence, GeneKeyTable geneKeys)
        {
            DisplayName = "Table";

            Layout = sequence
                .ToTable()
                .Select(row => row.Join(geneKeys, x => x.Number, x => x.Number, (_, k) => new TabularCell(k) { IsSelected = true }).ToArray())
                .ToArray();
        }

        public TabularCell[][] Layout { get; }

        public IEnumerable<TabularCell> AllCells => Layout.SelectMany(x => x);

        public void UpdateSelection(IHexagramFilter filter)
        {
            foreach (var cell in AllCells)
            {
                cell.IsSelected = filter.Includes(cell.Hexagram);
            }
        }
    }
}
