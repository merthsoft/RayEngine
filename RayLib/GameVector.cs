using System;
using System.Diagnostics;

namespace RayLib
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public record GameVector : IEquatable<GameVector>
    {
        public static GameVector Zero = new GameVector(0, 0);
        public static GameVector North = (0, -1);
        public static GameVector NorthEast = (1, -1);
        public static GameVector East = (1, 0);
        public static GameVector SouthEast = (1, 1);
        public static GameVector South = (0, -1);
        public static GameVector SouthWest = (-1, 1);
        public static GameVector West = (-1, 0);
        public static GameVector NorthWest = (-1, -1);

        public static GameVector[] CardinalDirections4 = new[] { East, North, West, South };
        public static GameVector[] CardinalDirections8 = new[] { East, NorthEast, North, NorthWest, West, SouthWest, South, SouthEast };

        public double X { get; }
        public double Y { get; }

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

        public double Atan2()
            => Y.Atan2(X);

        public GameVector ToCardinalDirection4()
            => (Atan2() + Math.PI) switch
            {
                var d when d < Math.PI / 4.0
                    => GameVector.East,
                var d when d < 3.0 * Math.PI / 4.0
                    => GameVector.North,
                var d when d < 5.0 * Math.PI / 4.0
                    => GameVector.West,
                var d when d < 7.0 * Math.PI / 4.0
                    => GameVector.South,
                _ => GameVector.East
            };

        public GameVector Floor()
            => (X.Floor(), Y.Floor());

        public GameVector Round()
            => (X.Round(), Y.Round());

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
