using System;

namespace Astrolo.Geometry
{
    public static class Seconds
    {
        public const int PerMinute = 60;
        public const int PerDegree = PerMinute * 60;
        public const int PerCircle = PerDegree * 360;

        public static int FromDegrees(int degrees, uint minutes = 0, uint seconds = 0)
        {
            if (minutes > 60)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes));
            }

            if (seconds > 60)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds));
            }

            var fraction = (int) (minutes * PerMinute + seconds);
            return degrees * PerDegree + (degrees > 0 ? fraction : -fraction);
        }

        public static Angle ToAngle(int seconds)
        {
            seconds %= PerCircle;
            return Angle.FromDegrees((seconds < 0 ? PerCircle + seconds : seconds) / (double)PerDegree);
        }
    }
}
