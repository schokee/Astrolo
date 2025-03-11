using System;
using System.Text.RegularExpressions;

namespace Astrolo.Geometry
{
    public readonly struct Angle(AngleUnitOfMeasure unitOfMeasure, double value) : IEquatable<Angle>
    {
        public const double DegreesPerRadian = 180 / Math.PI;

        private double Value { get; } = value;

        public AngleUnitOfMeasure UnitOfMeasure { get; } = unitOfMeasure;

        public bool IsMeasuredInRadians => UnitOfMeasure == AngleUnitOfMeasure.Radian;

        public bool IsMeasuredInDegrees => UnitOfMeasure == AngleUnitOfMeasure.Degree;

        public double Radians => IsMeasuredInRadians ? Value : Value / DegreesPerRadian;

        public double Degrees => IsMeasuredInDegrees ? Value : Value * DegreesPerRadian;

        public double Sin()
        {
            return Math.Sin(Radians);
        }

        public double Cos()
        {
            return Math.Cos(Radians);
        }

        public Angle ToRadians()
        {
            return To(AngleUnitOfMeasure.Radian);
        }

        public Angle ToDegrees()
        {
            return To(AngleUnitOfMeasure.Degree);
        }

        public Angle To(AngleUnitOfMeasure unitOfMeasure)
        {
            return new Angle(unitOfMeasure, unitOfMeasure == AngleUnitOfMeasure.Degree ? Degrees : Radians);
        }

        public bool Equals(Angle other)
        {
            return Degrees.Equals(other.Degrees);
        }

        public override bool Equals(object? obj)
        {
            return obj is Angle other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) UnitOfMeasure * 397) ^ Value.GetHashCode();
            }
        }

        public static Angle Zero => FromRadians(0);

        private static Regex CoordinatePattern { get; } = new(@"^\s*(?<deg>\d+)(°(?<min>\d{1,2})('(?<sec>\d{1,2})\"")?)?");

        public static Angle ParseCoordinate(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            var matchPattern = CoordinatePattern.Match(s);
            if (!matchPattern.Success) throw new FormatException("Failed to parse angle: " + s);

            var deg = double.Parse(matchPattern.Groups["deg"].Value);
            var min = matchPattern.Groups["min"];

            if (min.Success)
                deg += int.Parse(min.Value) / 60d;

            var sec = matchPattern.Groups["sec"];

            if (sec.Success)
                deg += int.Parse(sec.Value) / 3600d;

            return FromDegrees(deg);
        }

        public static Angle FromDegrees(double degrees, double minutes = 0, double seconds = 0)
        {
            return new Angle(AngleUnitOfMeasure.Degree, degrees + (minutes * 60 + seconds) / 3600);
        }

        public static Angle FromRadians(double value)
        {
            return new Angle(AngleUnitOfMeasure.Radian, value);
        }

        public static Angle Atan2(double y, double x)
        {
            return FromRadians(Math.Atan2(y, x));
        }

        public static Angle Asin(double d)
        {
            return FromRadians(Math.Asin(d));
        }

        public static Angle Acos(double d)
        {
            return FromRadians(Math.Acos(d));
        }

        public static bool operator ==(Angle left, Angle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Angle left, Angle right)
        {
            return !left.Equals(right);
        }

        public static Angle operator -(Angle a)
        {
            return new Angle(a.UnitOfMeasure, -a.Value);
        }

        public static Angle operator *(Angle a, double factor)
        {
            return new Angle(a.UnitOfMeasure, a.Value * factor);
        }

        public static Angle operator /(Angle a, double factor)
        {
            return new Angle(a.UnitOfMeasure, a.Value / factor);
        }

        public static Angle operator +(Angle a, Angle b)
        {
            return new Angle(a.UnitOfMeasure, a.Value + b.To(a.UnitOfMeasure));
        }

        public static Angle operator -(Angle a, Angle b)
        {
            return new Angle(a.UnitOfMeasure, a.Value - b.To(a.UnitOfMeasure).Value);
        }

        public static implicit operator double(Angle source)
        {
            return source.Value;
        }
    }
}
