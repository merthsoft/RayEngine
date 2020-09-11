using RayLib.Objects;

namespace RayLib.Intersections
{
    public record ObjectIntersection : Intersection
    {
        public GameObject Object { get; }

        public ObjectIntersection(GameObject @object, int screenX, int textureX, int top, int end, double distance, int spriteHeight, double angle)
            : base(@object.Def, screenX, textureX, top, end, distance, spriteHeight, angle)
            => Object = @object;

        public override RayTexture GetTexture()
            => Object.GetTexture(ViewAngleDegrees);
    }
}
