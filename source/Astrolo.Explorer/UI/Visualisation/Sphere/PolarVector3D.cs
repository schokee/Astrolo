using System.Windows.Media.Media3D;
using Astrolo.Geometry;

namespace Astrolo.Explorer.UI.Visualisation.Sphere;

public readonly struct PolarVector3D(Angle theta, Angle phi, double magnitude = 1)
{
    public double Magnitude { get; } = magnitude;

    public Angle Theta { get; } = theta;

    public Angle Phi { get; } = phi;

    public Vector3D ToVector()
    {
        return new Vector3D(Theta.Cos() * Phi.Sin(), Theta.Sin() * Phi.Sin(), Phi.Cos()) * Magnitude;
    }
}
