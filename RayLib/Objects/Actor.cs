using RayLib.Defs;
using System;

namespace RayLib.Objects
{
    public abstract class Actor : GameObject
    {
        protected Random Random = new Random();
        
        public int TextureIndex { get; set; }
        public ActorDef ActorDef => (Def as ActorDef)!;

        public int ActTimer { get; set; }

        public override bool Blocking => true;
        public override bool BlocksView => ActorDef.BlocksView;

        protected Actor()
            : base(ActorDef.Empty) { }

        public Actor(ActorDef def) 
            : base(def) { }

        public virtual void Act(Map map, Player player)
        {
            ActTimer--;
            if (ActTimer < 0)
                ActTimer = 0;
        }

        public override RayTexture GetTexture(double viewAngle)
            => GetTextureFromAngle(ActorDef.Textures[TextureIndex], viewAngle);
    }
}
