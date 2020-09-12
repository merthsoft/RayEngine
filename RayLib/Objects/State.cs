using System;

namespace RayLib.Objects
{
    public record State
    {
        public (int min, int max) Time { get; }
        public Func<Map, Player, bool> Action { get; }
        public (State, int)[] Next { get; }

        public State(int minTime, int maxTime, Func<Map, Player, bool> action, params (State, int)[] next)
        {
            Time = (minTime, maxTime);
            Action = action;
            Next = next;
        }
    }
}
