using System.Diagnostics;

namespace RayLib.Defs
{
    [DebuggerDisplay("{DefName}: {Name}")]
    public record Def
    {
        public static Def Empty { get; } = new("<EMPTY>", GameVector.Zero);

        public virtual string DefName => nameof(Def);

        public string Name { get; } = null!;
        public (int W, int H) DrawSize { get; }

        public Def(string name, (int, int) drawSize)
            => (Name, DrawSize)
             = (name, drawSize);

        public Def(string name, GameVector drawSize)
            : this(name, new((int)drawSize.X, (int)drawSize.Y)) { }
    }
}
