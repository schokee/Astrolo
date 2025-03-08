using Astrolo.Explorer.UI.Cleromancy;
using Astrolo.Explorer.UI.Profiling;
using Astrolo.Explorer.UI.Visualisation;
using Caliburn.Micro;

namespace Astrolo.Explorer.UI;

internal sealed class MainViewModel : Conductor<Screen>.Collection.OneActive
{
    public MainViewModel(ReadingViewModel reading, GeometryViewModel geometry, ProfileEditorViewModel profileEditor)
    {
        DisplayName = "Astrolo " + ProductInfo.Version;

        Items.Add(profileEditor);
        Items.Add(geometry);
        Items.Add(reading);

        ActiveItem = profileEditor;
    }
}
