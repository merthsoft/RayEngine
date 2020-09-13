using RayLib.Defs;

namespace RayLib.Objects
{
    public class Player : Actor
    {
        public override bool Blocking => true;
        public override bool BlocksView => false;
        
        public bool Interacting { get; set; }

        public Player(ActorDef def) : base(def)
        {

        }

        public override RayTexture GetTexture(double viewAngle)
            => RayTexture.Empty;
    }
}
