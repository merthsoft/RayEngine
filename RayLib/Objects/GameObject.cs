using RayLib.Defs;

namespace RayLib.Objects
{
    public abstract class GameObject
    {
        public Def Def { get; set; }

        public GameVector Location { get; set; } = GameVector.Zero;
        public GameVector Direction { get; set; } = (0, 0);
        public GameVector Plane { get; set; } = (0, 0.66f);

        public abstract bool Blocking { get; }
        public abstract bool BlocksView { get; }

        public GameObject(Def def)
            => Def = def;

        public abstract RayTexture GetTexture(double viewAngle);

        public GameObject SetLocation(GameVector v)
        {
            Location = v;
            return this;
        }

        public GameObject SetDirection(GameVector v)
        {
            Direction = v;
            return this;
        }

        public GameObject SetPlane(GameVector v)
        {
            Plane = v;
            return this;
        }
    }
}
