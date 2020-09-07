using RayLib.Intersections;
using RayLib.Objects;
using System.Collections.Generic;

namespace RayLib
{
    public record Step
    {
        public IEnumerable<Intersection> Intersections { get; }
        public IEnumerable<GameObject> ObjectsInSight { get; }
        public IEnumerable<double[]> ZBuffers { get; }


        public Step(IEnumerable<Intersection> intersections, IEnumerable<GameObject> objectsInSight, IEnumerable<double[]> zBuffers)
        {
            Intersections = intersections;
            ObjectsInSight = objectsInSight;
            ZBuffers = zBuffers;
        }
    }
}
