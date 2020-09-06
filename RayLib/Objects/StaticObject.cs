using RayLib.Defs;

namespace RayLib.Objects
{
    public class StaticObject : GameObject
    {
        public override bool Blocking => StaticObjectDef.Blocking;
        public StaticObjectDef StaticObjectDef => (Def as StaticObjectDef)!;

        public StaticObject(Def def, int x, int y) : base(def)
            => Location = (x + .5, y + .5);

        public override RayTexture GetTexture(double viewAngle)
            => StaticObjectDef.Texture;
    }
}
