using RayLib.Defs;
using System.Collections.Generic;

namespace RayLib.Objects
{
    public class StaticObject : GameObject
    {
        public override bool Blocking => StaticObjectDef.Blocking;
        public override bool BlocksView => StaticObjectDef.BlocksView;
        public StaticObjectDef StaticObjectDef => (Def as StaticObjectDef)!;
        public List<RayTexture> Textures => StaticObjectDef.Textures;

        public StaticObject(StaticObjectDef def, int x, int y) : base(def)
            => (Direction, Location)
             = (def.Direction, (x + .5, y + .5));

        public override RayTexture GetTexture(double viewAngle)
            => Direction == GameVector.Zero
             ? Textures[0]
             : GetTextureFromAngle(Textures, viewAngle);
    }
}
