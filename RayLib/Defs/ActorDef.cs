namespace RayLib.Defs
{
    public record ActorDef : Def
    {
        public new static ActorDef Empty { get; } = new("<EMPTY>", 0, 0);

        public override string DefName => nameof(ActorDef);

        public bool BlocksView { get; }
        public RayTexture[][] Textures { get; }

        public ActorDef(string name, bool blocksView, params RayTexture[][] textures)
            : base(name, textures[0][0].Size)
            => (Textures, BlocksView)
             = (textures, blocksView);

        protected ActorDef(string name, int w, int h)
            : base(name, w, h)
            => Textures = new[] { new RayTexture[0] };
    }
}
