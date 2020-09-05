using RayLib.Defs;

namespace RayLib.Objects
{
    public abstract class Actor : GameObject
    {
        public int TextureIndex { get; set; }
        public ActorDef ActorDef => (Def as ActorDef)!;
        public RayTexture CurrentTexture => ActorDef.Textures[TextureIndex];

        protected Actor()
            : base(ActorDef.Empty) { }

        public Actor(ActorDef def) : base(def)
        {

        }

        public abstract void Act(Map map);
    }
}
