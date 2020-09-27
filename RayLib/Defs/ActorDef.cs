namespace RayLib.Defs
{
    public record ActorDef : Def
    {
        public new static ActorDef Empty { get; } = new("<EMPTY>", GameVector.Zero);

        public override string DefName => nameof(ActorDef);

        public bool BlocksView { get; set; }
        public bool Blocking { get; set; } = true;
        public RenderStyle RenderStyle { get; set; } = RenderStyle.Sprite;
        public RayTexture[][] Textures { get; set; }

        public ActorDef(string name, bool blocking, bool blocksView, RenderStyle renderStyle, params RayTexture[][] textures)
            : base(name, textures[0][0].Size)
            => (Blocking, BlocksView, RenderStyle, Textures)
             = (blocking, blocksView, renderStyle, textures);

        public ActorDef(string name, params RayTexture[][] textures)
            : this(name, true, false, RenderStyle.Sprite, textures) { }

        public ActorDef(string name, GameVector drawSize)
            : base(name, drawSize)
            => Textures = new[] { System.Array.Empty<RayTexture>() };
    }
}
