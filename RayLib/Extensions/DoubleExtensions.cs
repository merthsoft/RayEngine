using System;

namespace RayLib.Extensions
{
    public static class DoubleExtensions
    {
        public static int CardinalDirection2IndexDegrees(this double angle)
            => angle switch
            {
                0 => 1,
                <= 45 => 0,
                90 => 0,
                < 135 => 1,
                180 => 1,
                <= 225 => 0,
                270 => 0,
                < 315 => 1,
                _ => 0
            };

        public static int CardinalDirection4IndexDegrees(this double angle) 
            => (int)(((angle + 30) % 360) / 60);
        
        public static int CardinalDirection8IndexDegrees(this double angle) 
            => (int)(((angle + 22.5) % 360) / 45);

        public static (double, double) Add(this (double, double) value, double additionalValue)
            => (value.Item1 + additionalValue, value.Item2 + additionalValue);

        public static double Round(this double value, int digits = 2)
            => Math.Round(value, digits, MidpointRounding.AwayFromZero);

        public static double RoundUp(this double value, int digits = 2)
            => value >= 0
             ? Math.Round(value, digits, MidpointRounding.AwayFromZero)
             : -Math.Round(value.Abs(), digits, MidpointRounding.AwayFromZero);

        public static double RoundUpToNearest(this double value, double roundTo) 
            => roundTo == 0 ? value : Math.Ceiling(value / roundTo) * roundTo;

        public static double RoundToNearestHalf(this double value)
            => value > 0
             ? value.RoundUpToNearest(.5)
             : -(value.Abs().RoundUpToNearest(.5));

        public static (double, double) RoundToNeastestHalf(this (double, double) value)
            => (value.Item1.RoundToNearestHalf(), value.Item2.RoundToNearestHalf());

        public static (double, double) Round(this (double, double) value, int digits = 2)
            => (value.Item1.Round(digits), value.Item2.Round(digits));

        public static double Cos(this double value)
            => Math.Cos(value);

        public static double Atan2(this double y, double x)
            => Math.Atan2(y, x);

        public static double Sin(this double value)
            => Math.Sin(value);

        public static double Abs(this double value)
            => Math.Abs(value);

        public static double Floor(this double value)
            => Math.Floor(value);

        public static (double, double) Floor(this (double, double) value)
            => (value.Item1.Floor(), value.Item2.Floor());

        public static double Ceiling(this double value)
            => Math.Ceiling(value);

        public static (double, double) Ceiling(this (double, double) value)
            => (value.Item1.Ceiling(), value.Item2.Ceiling());

        public static double ToDegrees(this double radians)
            => ((180 / Math.PI) * radians + 360) % 360;
    }
}
