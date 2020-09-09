using RayLib.Defs;

namespace RayLib.Objects
{
    public abstract class GameObject
    {
        public Def Def { get; set; }

        public GameVector Location { get; set; } = GameVector.Zero;
        public GameVector Direction { get; set; } = (0, 0);
        public double FieldOfView { get; set; } = 0.66;
        public GameVector Plane => -Direction.Perpendicularize() * FieldOfView;

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

        public GameObject SetFieldOfView(double fieldOfView)
        {
            FieldOfView = fieldOfView;
            return this;
        }
    }
}
