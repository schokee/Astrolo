using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Astrolo.Explorer.UI.Visualisation.Filtering;
using Astrolo.Presentation.Core.Input;
using Astrolo.Presentation.Core.Windows;
using Astrolo.YiJing;
using Caliburn.Micro;
using HelixToolkit.Wpf;

namespace Astrolo.Explorer.UI.Visualisation.Sphere
{
    public sealed class YiSphereViewModel : Screen, IDisposable
    {
        private readonly IDisposable _shutdown;

        private InspectionFilterViewModel _filter;
        private bool _showLabels;
        private int _selectedHexagram = 1;

        private IReadOnlyDictionary<int, Vector3D> ViewingVectors { get; }

        private LinesVisual3D PathVisual { get; } = new() { Color = Colors.DarkViolet, Thickness = 2 };

        public YiSphereViewModel(ISequence source)
        {
            DisplayName = "Yi Sphere";

            ViewingVectors = source.Hexagrams.ToDictionary(x => x.Number, x => x.Info.SphereLocation.ToVector(-3));
            Models = new BindableCollection<Visual3D>(source.GenerateVisuals().Prepend(PathVisual));

            ResetViewCommand = new DelegateCommand(_ => View?.ViewFrom(new Vector3D(1, -1, -1)));

            _shutdown = Observable
                .FromEventPattern(h => CompositionTarget.Rendering += h, h => CompositionTarget.Rendering -= h)
                .Subscribe(_ =>
                {
                    PathVisual.Points = Filter?.IsSelected == true && Filter.ProfileFilter.IsSelected && Filter.ProfileFilter.Profile != null
                        ? new Point3DCollection(Filter.ProfileFilter.Profile.Points.Select(x => x.GeneKey.Hexagram.Info.SphereLocation).ToLines(4.8))
                        : new Point3DCollection();
                });
        }

        public void Dispose()
        {
            _shutdown.Dispose();
        }

        public ICommand ResetViewCommand { get; }

        public BindableCollection<Visual3D> Models { get; }

        public IEnumerable<Visual3D> FilteredModels => Models.Where(IncludeVisual);

        public InspectionFilterViewModel Filter
        {
            get => _filter;
            set
            {
                void OnSelectionChanged(object sender, EventArgs e)
                {
                    UpdateModels();
                }

                if (_filter != null)
                    _filter.SelectionChanged -= OnSelectionChanged;

                _filter = value;

                if (_filter != null)
                    _filter.SelectionChanged += OnSelectionChanged;

                UpdateModels();
            }
        }

        public bool ShowLabels
        {
            get => _showLabels;
            set
            {
                if (Set(ref _showLabels, value))
                {
                    UpdateModels();
                }
            }
        }

        public int SelectedHexagram
        {
            get => _selectedHexagram;
            set
            {
                if (Set(ref _selectedHexagram, value))
                {
                    ResetView();
                }
            }
        }

        public async Task Export()
        {
            var filename = await IoC.Get<IFileDialogService>().SelectFileToSave(new FileDialogSettings
            {
                DefaultExtension = Exporters.DefaultExtension,
                Filter = Exporters.Filter,
                Title = "Save Sphere"
            });

            View?.SaveViewport(filename);
        }

        private IYiSphereView View => GetView() as IYiSphereView;

        private bool IncludeVisual(Visual3D visual)
        {
            var hexagram = YiSphere3D.GetHexagram(visual);
            return hexagram == null || (ShowLabels || !(visual is BillboardTextVisual3D)) && Filter.Includes(hexagram);
        }

        private void ResetView()
        {
            View?.ViewFrom(ViewingVectors[SelectedHexagram]);
        }

        private void UpdateModels()
        {
            NotifyOfPropertyChange(nameof(FilteredModels));
        }
    }
}
