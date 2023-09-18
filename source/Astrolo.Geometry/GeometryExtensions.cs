using System;
using System.Collections.Generic;

namespace Astrolo.Geometry
{
    public static class GeometryExtensions
    {
        public static double SmallestSize(this Size size)
        {
            return Math.Min(size.Width, size.Height);
        }

        public static double LargestSize(this Size size)
        {
            return Math.Max(size.Width, size.Height);
        }

        public static double SmallestSize(this Rectangle rectangle)
        {
            return Math.Min(rectangle.Width, rectangle.Height);
        }

        public static Rectangle ReduceBy(this Rectangle rectangle, double size)
        {
            return rectangle.ReduceBy(new Size(size, size));
        }

        public static Rectangle ReduceBy(this Rectangle rectangle, Size offset)
        {
            return new Rectangle(rectangle.TopLeft + offset, rectangle.BottomRight - offset);
        }

        public static IEnumerable<Point> EnumeratePointyTopVertices(this Hexagon hexagon)
        {
            var width = hexagon.Apotherm;
            var halfHeight = hexagon.Radius / 2;

            yield return new Point(0, -hexagon.Radius);
            yield return new Point(-width, -halfHeight);
            yield return new Point(-width, halfHeight);
            yield return new Point(0, hexagon.Radius);
            yield return new Point(width, halfHeight);
            yield return new Point(width, -halfHeight);
        }

        public static IEnumerable<Point> EnumerateFlatTopVertices(this Hexagon hexagon)
        {
            var height = hexagon.Apotherm;
            var halfWidth = hexagon.Radius / 2;

            yield return new Point(0, hexagon.Radius);
            yield return new Point(halfWidth, -height);
            yield return new Point(-halfWidth, -height);
            yield return new Point(-hexagon.Radius, 0);
            yield return new Point(-halfWidth, height);
            yield return new Point(halfWidth, height);
        }

        public static Point ToCartesianPoint(this PointyTop.Point point)
        {
            return new Point(Hexagon.UnitHeight * point.X + Hexagon.UnitHeight / 2 * point.Z, 3f * point.Z / 2);
        }
    }
}
