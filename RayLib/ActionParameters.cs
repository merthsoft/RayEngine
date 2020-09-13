using RayLib.Objects;

namespace RayLib
{
    public record ActionParameters
    {
        public Player Player { get; protected set; } = null!;
        public Actor Actor { get; protected set; } = null!;
        public Map Map { get; protected set; } = null!;

        public ActionParameters() { }

        public void Set(Player player, Actor actor, Map map)
            => (Player, Actor, Map)
             = (player, actor, map);
    }
}
