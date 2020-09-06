using RayLib.Defs;

namespace RayLib.Objects
{
    public class Player : GameObject
    {
        public Player(Def def) : base(def)
        {

        }

        public override RayTexture GetTexture(double viewAngle)
            => RayTexture.Empty;
    }
}
