using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System;
using System.Linq;

namespace RayEngine.Actors
{
    public class Sleeper : Follower
    {
        private int LookCounter { get; set; } = 25;
        private int AwakeCounter { get; set; } = 0;

        public Sleeper() : base() { }

        public Sleeper(ActorDef actorDef)
            : base(actorDef) { }

        public override void Act(Map map, Player player)
        {
            // TODO: State machine
            if (AwakeCounter == 0)
            {
                TextureIndex = 0;
                LookCounter--;
                if (LookCounter == 0)
                {
                    LookCounter = Random.Next(10, 70);
                    if (map.ObjectsInSight(this, (0, 0)).Contains(player))
                    {
                        TextureIndex = 1;
                        AwakeCounter = Random.Next(500, 525);
                    }
                }
            }
            else if (AwakeCounter > 480)
            {
                AwakeCounter--;
                TextureIndex = 1;
                MoveTimer = 1;
            }
            else if (AwakeCounter > 470)
            {
                AwakeCounter--;
                TextureIndex = 2;
                MoveTimer = 1;
            }
            else
            {
                TextureIndex = 2;
                base.Act(map, player);
                //if (map.ObjectsInSight(this, (0, 0)).Contains(player))
                //    AwakeCounter--;
            }
        }
    }
}
