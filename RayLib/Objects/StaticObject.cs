using RayLib.Defs;
using System.Collections.Generic;

namespace RayLib.Objects
{
    public class StaticObject : GameObject
    {
        public override bool Blocking => DirectionalStaticObjectDef.Blocking;
        public StaticObjectDef DirectionalStaticObjectDef => (Def as StaticObjectDef)!;
        public List<RayTexture> Textures => DirectionalStaticObjectDef.Textures;

        public StaticObject(StaticObjectDef def, int x, int y) : base(def)
            => (Direction, Location)
             = (def.Direction, (x + .5, y + .5));

        public override RayTexture GetTexture(double viewAngle)
            => Direction == GameVector.Zero
             ? Textures.Count == 1
                ? Textures[0]
                : Textures[(int)viewAngle % (360 / DirectionalStaticObjectDef.Textures.Count)]
             : Textures.Count switch
             {
                 2 => Textures[viewAngle.CardinalDirection2Index()],
                 4 => Textures[viewAngle.CardinalDirection4Index()],
                 8 => Textures[viewAngle.CardinalDirection8Index()],
                 _ => Textures[0],
             }
        ;
    }
}
