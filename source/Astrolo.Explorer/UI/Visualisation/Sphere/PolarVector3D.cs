using System.Windows.Media.Media3D;
using Astrolo.Geometry;

namespace Astrolo.Explorer.UI.Visualisation.Sphere;

public readonly struct PolarVector3D
{
    public PolarVector3D(Angle theta, Angle phi, double magnitude = 1)
    {
        Magnitude = magnitude;
        Theta = theta;
        Phi = phi;
    }

    public double Magnitude { get; }

    public Angle Theta { get; }

    public Angle Phi { get; }

    public Vector3D ToVector()
    {
        return new Vector3D(Theta.Cos() * Phi.Sin(), Theta.Sin() * Phi.Sin(), Phi.Cos()) * Magnitude;
    }
}
