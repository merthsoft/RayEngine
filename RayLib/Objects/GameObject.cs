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

        public GameObject(Def def)
            => Def = def;

        public abstract RayTexture GetTexture(double viewAngle);
    }
}
