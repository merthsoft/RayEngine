using RayLib.Defs;

namespace RayLib.Objects
{
    public class Player : Actor
    {
        public override bool Blocking => true;
        public override bool BlocksView => false;

        public Player(ActorDef def) : base(def)
        {

        }

        public override RayTexture GetTexture(double viewAngle)
            => RayTexture.Empty;

        public override void Act(Map map, Player player)
        {
        }
    }
}
