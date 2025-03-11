using System.Windows.Media.Media3D;
using Astrolo.Geometry;
using Astrolo.YiJing;
using HelixToolkit.Wpf;
using MoreLinq;

namespace Astrolo.Explorer.UI.Visualisation.Sphere;

public static class YiSphere3D
{
    private static IReadOnlyDictionary<Trigram, Color> Palette { get; } = new Dictionary<Trigram, Color>
    {
        { Trigram.Earth, Colors.DimGray },
        { Trigram.Mountain, Colors.DarkRed },
        { Trigram.Water, Colors.RoyalBlue },
        { Trigram.Wind, Colors.Gold },
        { Trigram.Thunder, Colors.YellowGreen },
        { Trigram.Fire, Colors.DarkOrange },
        { Trigram.Lake, Colors.BlueViolet },
        { Trigram.Heaven, Colors.LightGray }
    };

    #region Hexagram Property

    public static readonly DependencyProperty HexagramProperty = DependencyProperty.RegisterAttached(
        "Hexagram", typeof(HexagramFigure), typeof(YiSphere3D), new(default(HexagramFigure)));

    public static HexagramFigure GetHexagram(DependencyObject element)
    {
        return (HexagramFigure) element.GetValue(HexagramProperty);
    }

    public static void SetHexagram(DependencyObject element, HexagramFigure value)
    {
        element.SetValue(HexagramProperty, value);
    }

    #endregion

    public sealed class Geometry
    {
        public Geometry(double iconHeight = 1, double spacer = 1.5)
        {
            FigureSize = new(iconHeight * 0.9, iconHeight, iconHeight / (6 + SpacingFactor * 5));
            Radius = (iconHeight + spacer) * 6 / Math.PI;
        }

        public double SpacingFactor { get; } = 0.4;

        public Size3D FigureSize { get; }

        public double Radius { get; }
    }

    public static IEnumerable<Visual3D> GenerateVisuals(this ISequence source)
    {
        var geometry = new Geometry();

        var result = new List<Visual3D>(source.GenerateVisuals(geometry))
        {
            new SphereVisual3D {Radius = geometry.Radius * 0.98, Fill = new SolidColorBrush(Color.FromArgb(32, 255, 255, 255))},
            new SunLight(),
            new DirectionalHeadLight {Brightness = 0.3}
        };

        return result;
    }

    public static IEnumerable<Visual3D> GenerateVisuals(this ISequence source, Geometry geometry)
    {
        var palette = source.Trigrams.ToDictionary(x => x.Ordinal, x => MaterialHelper.CreateMaterial(Palette[x.Info.Key]));

        const double cxGap = 1.5;
        const int cxYinLine = 5;

        var figureSize = geometry.FigureSize;
        var cy = figureSize.Z;
        var cx = figureSize.X / (cxYinLine * 2 + cxGap);
        var xMid = cx * (cxYinLine + cxGap) / 2;

        foreach (var hexagram in source.Hexagrams)
        {
            var upperTrigram = new MeshBuilder(false, false);
            var lowerTrigram = new MeshBuilder(false, false);

            for (var i = 0; i < hexagram.LineCount; ++i)
            {
                var trigram = i < TrigramInfo.TotalLines ? lowerTrigram : upperTrigram;
                var yMid = cy * i * (1 + geometry.SpacingFactor) - (geometry.FigureSize.Y - cy) / 2;

                if (hexagram[i].IsYang)
                {
                    trigram.AddBox(new(0, yMid, 0), figureSize.X, figureSize.Z, figureSize.Z);
                }
                else
                {
                    trigram.AddBox(new(-xMid, yMid, 0), cx * cxYinLine, figureSize.Z, figureSize.Z);
                    trigram.AddBox(new(xMid, yMid, 0), cx * cxYinLine, figureSize.Z, figureSize.Z);
                }
            }

            var figure = new Model3DGroup();

            figure.Children.Add(new GeometryModel3D
            {
                Geometry = upperTrigram.ToMesh(true),
                Material = palette[hexagram.UpperTrigram.Ordinal]
            });


            figure.Children.Add(new GeometryModel3D
            {
                Geometry = lowerTrigram.ToMesh(true),
                Material = palette[hexagram.LowerTrigram.Ordinal]
            });

            figure.Transform = hexagram.Info.SphereLocation.ToTransform(geometry.Radius);

            var visual = (Visual3D) new ModelVisual3D { Content = figure };

            SetHexagram(visual, hexagram);
            yield return visual;

            visual = new BillboardTextVisual3D
            {
                Text = $"{hexagram.Number} {hexagram.Description}",
                Transform = hexagram.Info.SphereLocation.ToTransform(geometry.Radius * 1.2),
            };

            SetHexagram(visual, hexagram);
            yield return visual;
        }
    }

    public static IEnumerable<Point3D> ToLines(this IEnumerable<YiSpherePoint> source, double radius)
    {
        var origin = new Point3D();
        var points = source
            .Select(x => x.ToTransform(radius).Transform(origin))
            .Pairwise((a, b) => new[] { a, b })
            .SelectMany(x => x);

        return points;
    }

    public static Vector3D ToVector(this YiSpherePoint point, double radius)
    {
        return new PolarVector3D(Angle.FromDegrees(point.Angle), point.Elevation, radius * (3 - point.Sphere) / 3).ToVector();
    }

    public static Transform3D ToTransform(this YiSpherePoint point, double radius)
    {
        var displacement = point.ToVector(radius);
        var transform = new Transform3DGroup();

        transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new(1, 0, 0), point.Elevation.Degrees)));
        transform.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new(0, 0, 1), point.Angle + 90)));
        transform.Children.Add(new TranslateTransform3D(displacement.X, displacement.Y, point.Elevation.Cos() * radius));

        return transform;
    }
}
