using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System.Linq;

namespace RayEngine.Actors
{
    public class Follower : Actor
    {
        private int MoveTimer = 25;

        public Follower() : base() { }

        public Follower(ActorDef actorDef)
            : base(actorDef) { }

        public override void Act(Map map, Player player)
        {
            MoveTimer--;
            Direction = (player.Location - Location).ToCardinalDirection4();
            if (MoveTimer == 0)
            {
                MoveTimer = 25;
                var newLocation = map.FindPath(Location, player.Location + player.Direction).FirstOrDefault();
                if (newLocation == null)
                    newLocation = Location + Direction;
                if (newLocation != player.Location)
                    Location = newLocation;
            }
        }
    }
}
