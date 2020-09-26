using System;
using System.Collections.Generic;
using System.Linq;

namespace RayLib
{

    public record State<TActionParameters>
    {
        private readonly static Random Random = new Random();

        public (int min, int max) Time { get; }
        public Func<TActionParameters, bool> Action { get; }
        protected List<(State<TActionParameters> state, int weight)> Next { get; set; } = new();
        protected int TotalWeight { get; set; }

        public State(Func<TActionParameters, bool> action)
            : this(0, action) { }

        public State(int time, Func<TActionParameters, bool> action)
            : this(time, time, action) { }

        public State(int minTime, int maxTime, Func<TActionParameters, bool> action)
        {
            Time = (minTime, maxTime);
            Action = action;
        }

        public State<TActionParameters> AddNext(params State<TActionParameters>[] states)
        {
            foreach (var state in states)
                AddNext(state);
            return this;
        }

        public State<TActionParameters> AddNext(State<TActionParameters> state, int weight = 1)
        {
            TotalWeight += weight;
            Next.Add((state, TotalWeight));
            return this;
        }

        public State<TActionParameters> Chain(State<TActionParameters> state, int weight = 1)
        {
            AddNext(state, weight);
            return state;
        }

        public State<TActionParameters> Advance()
        {
            var nextState = Random.Next(TotalWeight);
            return Next.FirstOrDefault(t => nextState < t.weight).state ?? this;
        }

        public int GetTimer()
            => Random.Next(Time.min, Time.max);
    }

    public record State<TActionParameters, TData> : State<TActionParameters>
    {

        public TData Data { get; }

        public State(Func<TActionParameters, bool> action, TData data)
            : this(0, action, data) { }

        public State(int time, Func<TActionParameters, bool> action, TData data)
            : this(time, time, action, data) { }

        public State(int minTime, int maxTime, Func<TActionParameters, bool> action, TData data)
            : base(minTime, maxTime, action)
        {
            Data = data;
        }
    }
}
