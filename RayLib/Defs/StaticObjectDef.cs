using System.Collections.Generic;
using System.Linq;

namespace RayLib.Defs
{
    public record StaticObjectDef : Def
    {
        public override string DefName => nameof(StaticObjectDef);

        public bool Blocking { get; }
        public RenderStyle RenderStyle { get; } = RenderStyle.Sprite;
        public bool BlocksView { get; }
        public List<RayTexture> Textures { get; }
        public GameVector Direction { get; protected set; } = (0, 0);

        public StaticObjectDef(string name, bool blocking, bool blocksView, params RayTexture[] textures)
            : base(name, textures.First().Size)
        => (Blocking, BlocksView, Textures)
         = (blocking, blocksView, textures.ToList());

        public StaticObjectDef(string name, bool blocking, bool blocksView, GameVector direction, params RayTexture[] textures)
            : base(name, textures.First().Size)
        => (Blocking, BlocksView, Direction, Textures)
         = (blocking, blocksView, direction, textures.ToList());
    }
}
