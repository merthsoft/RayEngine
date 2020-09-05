using System.Collections.Generic;

namespace RayLib.Defs
{
    public record ActorDef : Def
    {
        public static new ActorDef Empty { get; } = new("<EMPTY>");

        public List<RayTexture> Textures { get; } = new();

        public ActorDef(string name) : base(name)
        {
        }
    }
}
