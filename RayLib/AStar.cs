using Priority_Queue;
using System;
using System.Collections.Generic;

namespace RayLib
{
    public interface INeighborable<T>
    {
        IEnumerable<T> GetNeighbors(T root);
    }

    // Largely from https://gist.github.com/keithcollins/307c3335308fea62db2731265ab44c06
    public static class AStar
    {
        public static List<TLocation> Search<TLocation, TObject>(TLocation start, TLocation goal, TObject obj, Func<TLocation, TLocation, double> cost)
            where TLocation : notnull
            where TObject : INeighborable<TLocation>
        {
            var cameFrom = new Dictionary<TLocation, TLocation>();
            var costSoFar = new Dictionary<TLocation, double>();
            
            var frontier = new SimplePriorityQueue<TLocation, double>();

            frontier.Enqueue(start, 0);

            cameFrom.Add(start, start);
            costSoFar.Add(start, 0);

            TLocation current;

            while (frontier.Count > 0)
            {
                current = frontier.Dequeue();

                if (current.Equals(goal)) 
                    break;

                foreach (var neighbor in obj.GetNeighbors(current))
                {
                    var newCost = costSoFar[current] + cost(current, neighbor);

                    if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                    {
                        costSoFar[neighbor] = newCost;
                        cameFrom[neighbor] = current;
                        var priority = newCost + cost(neighbor, goal);
                        frontier.Enqueue(neighbor, priority);
                    }
                }
            }

            var path = new List<TLocation>();
            current = goal;

            while (!current.Equals(start))
            {
                path.Add(current);
                if (!cameFrom.ContainsKey(current))
                    return new();
                current = cameFrom[current];
            }

            path.Reverse();
            return path;
        }
    }
}
