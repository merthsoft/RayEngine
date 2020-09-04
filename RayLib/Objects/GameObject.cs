using RayLib.Defs;

namespace RayLib
{
    public class GameObject
    {
        public Def Def { get; }

        public Vector2d Location { get; set; } = Vector2d.Zero;
        public Vector2d Direction { get; set; } = Vector2d.Zero;
        public Vector2d Plane { get; set; } = (0, 0.66f);

        public GameObject(Def def)
            => Def = def;
    }
}
