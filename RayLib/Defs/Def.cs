using System.Diagnostics;

namespace RayLib.Defs
{
    [DebuggerDisplay("{DefName}: {Name}")]
    public record Def
    {
        public static Def Empty { get; } = new("<EMPTY>");

        public virtual string DefName => nameof(Def);
        public string Name { get; }

        public Def(string name)
            => Name = name;
    }
}
