namespace RayLib.Defs
{
    public record StaticObjectDef : Def
    {
        public RayTexture Texture { get; } = null!;

        public StaticObjectDef(string name, RayTexture texture)
            : base(name, texture.Size)
        => Texture = texture;
    }
}
