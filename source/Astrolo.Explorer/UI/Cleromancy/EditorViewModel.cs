using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using Astrolo.Explorer.Services;
using Astrolo.GeneKeys;
using Astrolo.YiJing;
using Caliburn.Micro;
using CommunityToolkit.Mvvm.Input;

namespace Astrolo.Explorer.UI.Cleromancy
{
    public sealed class EditorViewModel : PropertyChangedBase
    {
        private IReadOnlyList<Change> _changes;

        private GeneKeyTable Source { get; }

        public EditorViewModel(GeneKeyTable source, IEnumerable<Movement> changes, IYiJingBrowser browser)
        {
            Source = source;
            Changes = EnumerateChanges(changes);

            ComplementCommand = new RelayCommand<HexagramFigure>(_ => EndId = SelectGeneKey(StartState!.Hexagram.Complement).Number);
            FlipCommand = new RelayCommand<HexagramFigure>(_ => EndId = SelectGeneKey(StartState!.Hexagram.Invert).Number);
            SwapCommand = new RelayCommand(Swap);
            ViewInfoCommand = new RelayCommand<HexagramFigure>(browser.ViewInfo);
            ViewReadingCommand = new RelayCommand(() => browser.ViewReading(EnumerateMovements()));
        }

        public ICommand FlipCommand { get; }
        public ICommand SwapCommand { get; }
        public ICommand ComplementCommand { get; }

        public ICommand ViewInfoCommand { get; }
        public ICommand ViewReadingCommand { get; }

        public IReadOnlyList<Change> Changes
        {
            get => _changes;
            private set
            {
                if (Equals(value, _changes)) return;

                void OnStateChanged(object sender, EventArgs e)
                {
                    UpdateState();
                }

                if (_changes != null)
                {
                    foreach (var change in _changes)
                    {
                        change.StateChanged -= OnStateChanged;
                    }
                }

                _changes = value;

                if (_changes != null)
                {
                    foreach (var change in _changes)
                    {
                        change.StateChanged += OnStateChanged;
                    }
                }

                NotifyOfPropertyChange();
                UpdateState();
            }
        }

        [Range(1, HexagramInfo.TotalHexagrams)]
        public int StartId
        {
            get => StartState.Number;
            set
            {
                if (value == StartId) return;

                StartState = Source[value];
                Changes = EnumerateChanges();
            }
        }

        [Range(1, HexagramInfo.TotalHexagrams)]
        public int EndId
        {
            get => EndState.Number;
            set
            {
                if (value == EndId) return;

                EndState = Source[value];
                Changes = EnumerateChanges();
            }
        }

        public IGeneKey StartState { get; private set; }

        public IGeneKey EndState { get; private set; }

        public Progress Progress { get; private set; }

        private IGeneKey SelectGeneKey(Func<Change, Line> selectLine)
        {
            var ordinal = Changes!.Select(selectLine).Reverse().Aggregate(0u, (result, l) => (result << 1) | l);
            return SelectGeneKey(ordinal);
        }

        private IGeneKey SelectGeneKey(HexagramFigure hexagram)
        {
            return SelectGeneKey(hexagram.Ordinal);
        }

        private IGeneKey SelectGeneKey(uint ordinal)
        {
            return Source.First(x => x.Hexagram.Ordinal == ordinal);
        }

        private IReadOnlyList<Change> EnumerateChanges()
        {
            return EnumerateChanges(EnumerateMovements());
        }

        private IEnumerable<Movement> EnumerateMovements()
        {
            return StartState.Hexagram.Diff(EndState.Hexagram);
        }

        private void Swap()
        {
            (EndId, StartId) = (StartId, EndId);
        }

        private void UpdateState()
        {
            StartState = SelectGeneKey(x => x.StartState);
            NotifyOfPropertyChange(nameof(StartState));
            NotifyOfPropertyChange(nameof(StartId));

            EndState = SelectGeneKey(x => x.EndState);
            NotifyOfPropertyChange(nameof(EndState));
            NotifyOfPropertyChange(nameof(EndId));

            var totalMoves = Changes?.Count(x => x.IsMoving);

            Progress = totalMoves < TrigramInfo.TotalLines ? Progress.Start : totalMoves > TrigramInfo.TotalLines ? Progress.End : Progress.Middle;
            NotifyOfPropertyChange(nameof(Progress));
        }

        private static IReadOnlyList<Change> EnumerateChanges(IEnumerable<Movement> changes)
        {
            return changes.Select(x => new Change(x)).ToList();
        }
    }
}
