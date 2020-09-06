namespace RayLib.Defs
{
    public record StaticObjectDef : Def
    {
        public bool Blocking { get; }
        public RayTexture Texture { get; }

        public StaticObjectDef(string name, bool blocking, RayTexture texture)
            : base(name, texture.Size)
        => (Blocking, Texture)
         = (blocking, texture);
    }
}
