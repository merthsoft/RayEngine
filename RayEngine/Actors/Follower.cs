using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System;
using System.Linq;

namespace RayEngine.Actors
{
    public class Follower : Actor
    {
        protected int MoveTimer = 300;

        public Follower() : base() { }

        public Follower(ActorDef actorDef)
            : base(actorDef) { }

        public override void Act(Map map, Player player)
        {
            var gamePlayer = player as GamePlayer;
            if (gamePlayer == null)
                return;

            MoveTimer--;
            Direction = GameVector.CardinalDirections8[(Location - gamePlayer.Location).Atan2().ToDegrees().CardinalDirection8IndexDegrees()];
            if (MoveTimer == 0)
            {
                MoveTimer = Random.Next(10, 50);
                var newLocation = map.FindPath(Location, gamePlayer.Location).FirstOrDefault();
                if (newLocation == null)
                    newLocation = Location + Direction;

                if (newLocation == gamePlayer.Location)
                {
                    gamePlayer.ScreenFlash = (64, 0, 0);
                    gamePlayer.ScreenFlashDuration += 50;
                    gamePlayer.ScreenFlashDecay = 1;
                }

                if (map.BlockedAt(0, newLocation))
                    newLocation = Location - Direction;

                if (!map.BlockedAt(0, newLocation))
                    Location = newLocation;
            }
        }
    }
}
