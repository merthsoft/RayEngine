using System;
using RayLib.Objects;

namespace RayLib
{
    public record State<TData>
    {
        public (int min, int max) Time { get; }
        public Func<Map, Player, bool> Action { get; }
        public (State<TData>, int)[] Next { get; }
        public TData Data { get; }

        public State(int minTime, int maxTime, Func<Map, Player, bool> action, TData data, params (State<TData>, int)[] next)
        {
            Time = (minTime, maxTime);
            Action = action;
            Next = next;
            Data = data;
        }
    }
}
