using System;
using System.Diagnostics;

namespace RayLib
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public record GameVector : IEquatable<GameVector>
    {
        public static GameVector Zero = new GameVector(0, 0);
        
        public static GameVector North = (0, 1);
        public static GameVector NorthEast = (.71, .71);
        public static GameVector East = (1, 0);
        public static GameVector SouthEast = (.71, -.71);
        public static GameVector South = (0, -1);
        public static GameVector SouthWest = (-.71, -.71);
        public static GameVector West = (-1, 0);
        public static GameVector NorthWest = (-.71, .71);

        public static GameVector[] CardinalDirections4 = new[] { East, North, West, South };
        public static GameVector[] CardinalDirections8 = new[] { East, NorthEast, North, NorthWest, West, SouthWest, South, SouthEast };

        public static string[] CardinalDirections4Names = new[] { "East", "North", "West", "South" };
        public static string[] CardinalDirections8Names = new[] { "East", "NorthEast", "North", "NorthWest", "West", "SouthWest", "South", "SouthEast" };

        public double X { get; }
        public double Y { get; }

        public double Angle => Atan2().ToDegrees();
        public int CardinalDirection4Index => Angle.CardinalDirection4IndexDegrees();
        public int CardinalDirection8Index => Angle.CardinalDirection8IndexDegrees();

        public static GameVector operator +(GameVector lhs, GameVector rhs)
            => (lhs.X + rhs.X, lhs.Y + rhs.Y);

        public static GameVector operator -(GameVector lhs, GameVector rhs)
            => (lhs.X - rhs.X, lhs.Y - rhs.Y);

        public static GameVector operator -(GameVector v)
            => (-v.X, -v.Y);

        public static GameVector operator /(GameVector lhs, double rhs)
            => (lhs.X / rhs, lhs.Y / rhs);

        public static GameVector operator *(GameVector lhs, double rhs)
            => (lhs.X * rhs, lhs.Y * rhs);

        public static GameVector operator /(GameVector lhs, int rhs)
            => (lhs.X / rhs, lhs.Y / rhs);

        public static GameVector operator /(int lhs, GameVector rhs)
            => (rhs.X == 0 ? double.PositiveInfinity : lhs / rhs.X, 
                rhs.Y == 0 ? double.PositiveInfinity : lhs / rhs.Y);

        public static GameVector operator *(GameVector lhs, int rhs)
            => (lhs.X * rhs, lhs.Y * rhs);

        public static GameVector operator *(int lhs, GameVector rhs)
            => (lhs * rhs.X, lhs * rhs.Y);

        public static GameVector operator +(GameVector lhs, double rhs)
            => (lhs.X + rhs, lhs.Y + rhs);

        public static GameVector operator +(double lhs, GameVector rhs)
            => (lhs + rhs.X, lhs + rhs.Y);

        public static bool operator ==(GameVector lhs, (double x, double y) rhs)
            => lhs.X == rhs.x
            && lhs.Y == rhs.y;

        public static bool operator !=(GameVector lhs, (double x, double y) rhs)
            => lhs.X != rhs.x
            && lhs.Y != rhs.y;

        public static implicit operator GameVector((double x, double y) t)
            => new GameVector(t.x, t.y);

        public void Deconstruct(out double x, out double y)
            => (x, y) 
             = (X, Y);

        public GameVector()
            => (X, Y) = (0, 0);

        public GameVector(double x, double y)
            => (X, Y) 
             = (x, y);
        
        public virtual bool Equals(GameVector? other) 
            => X == other?.X 
            && Y == other?.Y;
        
        public override int GetHashCode() 
            => HashCode.Combine(X, Y);

        private string GetDebuggerDisplay()
            => $"({X}, {Y})";

        public GameVector Abs()
            => (X.Abs(), Y.Abs());

        public GameVector Perpendicularize()
            => (Y, -X);

        public double Atan2()
            => Y.Atan2(X);

        public GameVector Floor()
            => (X.Floor(), Y.Floor());

        public GameVector Round(int digits = 2)
            => (X.Round(digits), Y.Round(digits));

        public double Distance(GameVector other)
            => Math.Sqrt(this.UnscaledDistance(other));

        public double UnscaledDistance(GameVector other)
            => ((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));

        public override string ToString()
            => $"({X}, {Y})";

        public GameVector MoveForward(GameVector direction, double speed)
            => this + direction * speed;
    }
}
