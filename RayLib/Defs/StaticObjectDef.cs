using System.Collections.Generic;
using System.Linq;

namespace RayLib.Defs
{
    public record StaticObjectDef : Def
    {
        public bool Blocking { get; }
        public List<RayTexture> Textures { get; }
        public GameVector Direction { get; protected set; } = (0, 0);

        public StaticObjectDef(string name, bool blocking, params RayTexture[] textures)
            : base(name, textures.First().Size)
        => (Blocking, Textures)
         = (blocking, textures.ToList());

        public StaticObjectDef(string name, bool blocking, GameVector direction, params RayTexture[] textures)
            : base(name, textures.First().Size)
        => (Blocking, Direction, Textures)
         = (blocking, direction, textures.ToList());
    }
}
