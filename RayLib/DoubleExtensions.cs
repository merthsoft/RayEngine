using System;

namespace RayLib
{
    public static class DoubleExtensions
    {
        public static double Round(this double f, int digits = 2)
            => Math.Round(f, digits);

        public static (double, double) Round(this (double f1, double f2) f, int digits = 2)
            => (f.f1.Round(digits), f.f2.Round(digits));

        public static double Cos(this double f)
            => Math.Cos(f);

        public static double Atan2(this double y, double x)
            => Math.Atan2(y, x);

        public static double Sin(this double f)
            => Math.Sin(f);

        public static double Abs(this double f)
            => Math.Abs(f);

        public static double Floor(this double f)
            => Math.Floor(f);
    }
}
