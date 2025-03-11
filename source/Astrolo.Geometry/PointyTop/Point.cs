using System;
using System.Collections.Generic;
using System.Linq;

namespace Astrolo.Geometry.PointyTop
{
    // See also: http://www.redblobgames.com/grids/hexagons/

    public readonly struct Point(int x, int y, int z) : IEquatable<Point>
    {
        public static Point Origin { get; } = new();

        private static class Unit
        {
            public static Point[] Diagonal { get; } =
            [
                new(1, -2, 1),
                new(-1, -1, 2),
                new(-2, 1, 1),
                new(-1, 2, -1),
                new(1, 1, -2),
                new(2, -1, -1)
            ];

            public static Point[] Step { get; } =
            [
                new( 1, -1,  0 ),
                new( 0, -1,  1 ),
                new(-1,  0,  1 ),
                new(-1,  1,  0 ),
                new( 0,  1, -1 ),
                new( 1,  0, -1 )
            ];
        }

        public static IEnumerable<Face> AllFaces => Enum.GetValues(typeof(Face)).Cast<Face>();

        private static IEnumerable<Vertex> AllCorners => Enum.GetValues(typeof(Vertex)).Cast<Vertex>();

        public int X { get; } = x;
        public int Y { get; } = y;
        public int Z { get; } = z;

        public int DistanceTo(Point other)
        {
            var dx = Math.Abs(X - other.X);
            var dy = Math.Abs(Y - other.Y);
            var dz = Math.Abs(Z - other.Z);

            return Math.Max(Math.Max(dx, dy), dz);
        }

        public Point Scale(double factor)
        {
            return Round(X * factor, Y * factor, Z * factor);
        }

        public Point NeighborAt(Face face)
        {
            return this + Unit.Step[(int)face];
        }

        public Point DiagonalNeighbourAt(Vertex vertex)
        {
            return this + Unit.Diagonal[(int)vertex];
        }

        public IEnumerable<Point> Neighbours => AllFaces.Select(NeighborAt);

        public IEnumerable<Point> DiagonalNeighbours => AllCorners.Select(DiagonalNeighbourAt);

        public IEnumerable<Point> EnumeratePerimeter(int size, Face startingFace = Face.Right)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

            var neighboringPoint = NeighborAt(startingFace).Scale(size);
            var traversalSequence = AllFaces.Concat(AllFaces).Skip(startingFace - Face.Right + 2).Take(6);

            foreach (var face in traversalSequence)
            {
                for (var i = 0; i < size; ++i)
                {
                    yield return neighboringPoint;
                    neighboringPoint = neighboringPoint.NeighborAt(face);
                }
            }
        }

        public override string ToString()
        {
            return $"X={X} Y={Y} Z={Z}";
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }

        public static Point operator *(Point p, double factor)
        {
            return p.Scale(factor);
        }

        public static Point operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Point operator -(Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }

        private static Point Round(double x, double y, double z)
        {
            var rx = (int)Math.Round(x);
            var xDiff = Math.Abs(rx - x);

            var ry = (int)Math.Round(y);
            var yDiff = Math.Abs(ry - y);

            var rz = (int)Math.Round(z);
            var zDiff = Math.Abs(rz - z);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                rx = -ry - rz;
            }
            else if (yDiff > zDiff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new Point(rx, ry, rz);
        }
    }
}
