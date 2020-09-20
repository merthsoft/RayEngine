using RayLib.Intersections;
using RayLib.Objects;
using System.Collections.Generic;

namespace RayLib
{
    public record Step
    {
        public ICollection<Intersection> Intersections { get; }
        public ICollection<GameObject> ObjectsInSight { get; }
        public ICollection<double[]> ZBuffers { get; }


        public Step(ICollection<Intersection> intersections, ICollection<GameObject> objectsInSight, ICollection<double[]> zBuffers)
        {
            Intersections = intersections;
            ObjectsInSight = objectsInSight;
            ZBuffers = zBuffers;
        }
    }
}
