namespace RayLib.Defs
{
    public record ActorDef : Def
    {
        public new static ActorDef Empty { get; } = new("<EMPTY>", 0, 0);

        public override string DefName => nameof(ActorDef);

        public bool BlocksView { get; set; }
        public bool Blocking { get; set; } = true;
        public RenderStyle RenderStyle { get; set; } = RenderStyle.Sprite;
        public RayTexture[][] Textures { get; set; }

        public ActorDef(string name, params RayTexture[][] textures)
            : base(name, textures[0][0].Size)
            => Textures = textures;

        protected ActorDef(string name, int w, int h)
            : base(name, w, h)
            => Textures = new[] { System.Array.Empty<RayTexture>() };
    }
}
