using System;
using System.Diagnostics;

namespace RayLib
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public record Vector2d : IEquatable<Vector2d>
    {
        public static Vector2d Zero = new Vector2d(0, 0);
        public double X { get; }
               
        public double Y { get; }

        public static Vector2d operator +(Vector2d lhs, Vector2d rhs)
            => (lhs.X + rhs.X, lhs.Y + rhs.Y);

        public static Vector2d operator -(Vector2d lhs, Vector2d rhs)
            => (lhs.X - rhs.X, lhs.Y - rhs.Y);

        public static Vector2d operator -(Vector2d v)
            => (-v.X, -v.Y);

        public static Vector2d operator /(Vector2d lhs, double rhs)
            => (lhs.X / rhs, lhs.Y / rhs);

        public static Vector2d operator *(Vector2d lhs, double rhs)
            => (lhs.X * rhs, lhs.Y * rhs);

        public static Vector2d operator /(Vector2d lhs, int rhs)
            => (lhs.X / rhs, lhs.Y / rhs);

        public static Vector2d operator /(int lhs, Vector2d rhs)
            => (rhs.X == 0 ? double.PositiveInfinity : lhs / rhs.X, 
                rhs.Y == 0 ? double.PositiveInfinity : lhs / rhs.Y);

        public static Vector2d operator *(Vector2d lhs, int rhs)
            => (lhs.X * rhs, lhs.Y * rhs);

        public static Vector2d operator *(int lhs, Vector2d rhs)
            => (lhs * rhs.X, lhs * rhs.Y);

        public static bool operator ==(Vector2d lhs, (double x, double y) rhs)
            => lhs.X == rhs.x
            && lhs.Y == rhs.y;

        public static bool operator !=(Vector2d lhs, (double x, double y) rhs)
            => lhs.X != rhs.x
            && lhs.Y != rhs.y;

        public static implicit operator Vector2d((double x, double y) t)
            => new Vector2d(t.x, t.y);

        public void Deconstruct(out double x, out double y)
            => (x, y) 
             = (X, Y);

        public Vector2d()
            => (X, Y) = (0, 0);

        public Vector2d(double x, double y)
            => (X, Y) 
             = (x, y);
        
        public virtual bool Equals(Vector2d? other) 
            => X == other?.X 
            && Y == other?.Y;
        
        public override int GetHashCode() 
            => HashCode.Combine(X, Y);

        private string GetDebuggerDisplay()
            => $"({X}, {Y})";

        public Vector2d Abs()
            => (X.Abs(), Y.Abs());

        public double Atan2()
            => Y.Atan2(X);

        public Vector2d Floor()
            => (X.Floor(), Y.Floor());

        public Vector2d Round()
            => (X.Round(), Y.Round());

        public double Distance(Vector2d other)
            => Math.Sqrt(this.UnscaledDistance(other));

        public double UnscaledDistance(Vector2d other)
            => ((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));

        public override string ToString()
            => $"({X}, {Y})";
    }
}
