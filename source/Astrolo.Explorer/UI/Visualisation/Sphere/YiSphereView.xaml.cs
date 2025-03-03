using System.Windows.Media.Media3D;

namespace Astrolo.Explorer.UI.Visualisation.Sphere;

public partial class YiSphereView : IYiSphereView
{
    public YiSphereView()
    {
        InitializeComponent();
    }

    public void ViewFrom(Vector3D vector)
    {
        Viewport.FitView(vector /*new Vector3D(1, -1, -1)*/, new(0, 0, 1), 500);
    }

    public void SaveViewport(string filename)
    {
        Viewport.Export(filename);
    }
}

public interface IYiSphereView
{
    void ViewFrom(Vector3D vector);

    void SaveViewport(string filename);
}
