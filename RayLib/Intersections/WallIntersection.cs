using RayLib.Defs;
using System;

namespace RayLib.Intersections
{
    public record WallIntersection : Intersection
    {
        public WallDef WallDef => (WallDef)Def;
        public bool NorthWall { get; }

        public WallIntersection(Def def, int screenX, int textureX, int top, int end, double distance, int lineHeight, double angle)
            : base(def, screenX, textureX, top, end, distance, lineHeight, angle)
            => NorthWall = angle < Math.PI;

        public override RayTexture GetTexture()
            => NorthWall ? WallDef.NorthSouthTexture : WallDef.EastWestTexture;
    }
}
