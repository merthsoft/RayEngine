using RayLib.Defs;
using System.Collections.Generic;

namespace RayLib.Objects
{
    public class StaticObject : GameObject
    {
        public override bool Blocking => StaticObjectDef.Blocking;
        public override bool BlocksView => StaticObjectDef.BlocksView;
        public override RenderStyle RenderStyle => StaticObjectDef.RenderStyle;
        public StaticObjectDef StaticObjectDef => (Def as StaticObjectDef)!;
        public List<RayTexture> Textures => StaticObjectDef.Textures;

        public StaticObject(StaticObjectDef def, double x, double y) : base(def)
            => (Direction, Location)
             = (def.Direction, (x, y));

        public override RayTexture GetTexture(double viewAngle)
            => Direction == GameVector.Zero
             ? Textures[0]
             : GetTextureFromAngle(Textures, viewAngle);
    }
}
