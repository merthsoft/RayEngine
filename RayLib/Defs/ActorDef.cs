using System.Collections.Generic;
using System.Linq;

namespace RayLib.Defs
{
    public record ActorDef : Def
    {
        public static new ActorDef Empty { get; } = new("<EMPTY>", 0, 0);

        public List<RayTexture> Textures { get; } = new();

        public ActorDef(string name, params RayTexture[] textures)
            : base(name, textures.First().Size)
        {
            Textures = textures.ToList();
        }

        protected ActorDef(string name, int w, int h)
            : base(name, w, h)
        {
        }
    }
}
