using RayLib.Defs;

namespace RayLib.Objects
{
    public abstract class GameObject
    {
        public Def Def { get; set; }

        public Vector2d Location { get; set; } = Vector2d.Zero;
        public Vector2d Direction { get; set; } = (1, 0);
        public Vector2d Plane { get; set; } = (0, 0.66f);

        public abstract bool Blocking { get; }

        public GameObject(Def def)
            => Def = def;

        public abstract RayTexture GetTexture(double viewAngle);
    }
}
