using RayLib.Defs;
using System;

namespace RayLib.Objects
{
    public abstract class Actor : GameObject
    {
        protected Random Random = new Random();
        
        public int TextureIndex { get; set; }
        public ActorDef ActorDef => (Def as ActorDef)!;

        public override bool Blocking => true;
        public override bool BlocksView => ActorDef.BlocksView;

        protected Actor()
            : base(ActorDef.Empty) { }

        public Actor(ActorDef def) : base(def)
        {

        }

        public abstract void Act(Map map, Player player);
        public override RayTexture GetTexture(double viewAngle) => ActorDef.Textures[TextureIndex];
    }
}
