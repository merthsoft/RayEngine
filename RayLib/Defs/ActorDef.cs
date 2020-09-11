using System.Collections.Generic;
using System.Linq;

namespace RayLib.Defs
{
    public record ActorDef : Def
    {
        public new static ActorDef Empty { get; } = new("<EMPTY>", 0, 0);

        public override string DefName => nameof(ActorDef);

        public bool BlocksView { get; }
        public List<RayTexture> Textures { get; } = new();

        public ActorDef(string name, bool blocksView, params RayTexture[] textures)
            : base(name, textures.First().Size)
        {
            Textures = textures.ToList();
            BlocksView = blocksView;
        }

        protected ActorDef(string name, int w, int h)
            : base(name, w, h)
        {
        }
    }
}
