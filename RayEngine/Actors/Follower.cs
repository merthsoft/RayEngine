using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System;
using System.Linq;

namespace RayEngine.Actors
{
    public class Follower : Actor
    {
        private Random Random = new Random();
        private int MoveTimer = 300;

        public Follower() : base() { }

        public Follower(ActorDef actorDef)
            : base(actorDef) { }

        public override void Act(Map map, Player player)
        {
            MoveTimer--;
            Direction = GameVector.CardinalDirections8[(player.Location - Location).CardinalDirection8Index];
            if (MoveTimer == 0)
            {
                MoveTimer = Random.Next(10, 50);
                var newLocation = map.FindPath(Location, player.Location).FirstOrDefault();
                if (newLocation == null)
                    newLocation = Location + Direction;

                if (newLocation == player.Location)
                {
                    player.ScreenFlash = (64, 0, 0);
                    player.ScreenFlashDuration += 50;
                    player.ScreenFlashDecay = 1;
                }

                if (map.BlockedAt(0, newLocation))
                    newLocation = Location - Direction;
                if (!map.BlockedAt(0, newLocation))
                    Location = newLocation;
            }
        }
    }
}
