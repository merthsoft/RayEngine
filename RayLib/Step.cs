using RayLib.Intersections;
using RayLib.Objects;
using System.Collections.Generic;

namespace RayLib
{
    public record Step
    {
        public IEnumerable<Intersection> Intersections { get; }
        public IEnumerable<GameObject>?[,] ObjectMap { get; }
        public IEnumerable<GameObject> ObjectsInSight { get; }
        public IEnumerable<double[]> ZBuffers { get; }

        public Step(IEnumerable<Intersection> intersections, IEnumerable<GameObject>?[,] objectMap, IEnumerable<GameObject> objectsInSight, IEnumerable<double[]> zBuffers)
        {
            Intersections = intersections;
            ObjectMap = objectMap;
            ObjectsInSight = objectsInSight;
            ZBuffers = zBuffers;
        }
    }
}
