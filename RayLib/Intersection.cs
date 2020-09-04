using RayLib.Defs;

namespace RayLib
{
    public abstract record Intersection
    {
        public Def Def { get; }
        public int ScreenX { get; }
        public int TextureX { get; }
        public int Top { get; }
        public int Bottom { get; }
        public double Distance { get; }
        public int Height { get; }

        protected Intersection(Def def, int screenX, int textureX, int top, int bottom, double distance, int height)
        {
            Def = def;
            ScreenX = screenX;
            TextureX = textureX;
            Top = top;
            Bottom = bottom;
            Distance = distance;
            Height = height;
        }
    }

    public record WallIntersection : Intersection
    {
        public WallDef WallDef => (WallDef)Def;
        public bool NorthWall { get; }

        public WallIntersection(Def def, int screenX, int textureX, int top, int end, double distance, int lineHeight, bool northWall) 
            : base(def, screenX, textureX, top, end, distance, lineHeight)
            => NorthWall = northWall;
    }

    public record ObjectIntersection : Intersection
    {
        public GameObject Object { get; }

        public ObjectIntersection(GameObject @object, int screenX, int textureX, int top, int end, double distance, int spriteHeight)
            : base(@object.Def, screenX, textureX, top, end, distance, spriteHeight)
            => Object = @object;
    }
}
