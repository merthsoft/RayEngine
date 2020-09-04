using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayLib.Defs
{
    public record StaticObjectDef : Def
    {
        public RayTexture Texture { get; } = null!;

        public StaticObjectDef(string name, RayTexture texture)
            : base(name)
        => Texture = texture;
    }
}
