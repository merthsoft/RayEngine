using RayLib.Defs;
using RayLib.Objects;

namespace RayEngine.Actors
{
    public class Wanderer : Actor
    {
        // TODO: Make into ActTimer
        //private int WalkTimer = 15;
        //private int TurnTimer = 25;

        public Wanderer() : base() { }

        public Wanderer(ActorDef actorDef)
            : base(actorDef) { }

        public override void Act(RayLib.ActionParameters parameters)
        {
            base.Act(parameters);
            //TurnTimer--;
            //WalkTimer--;

            //(var posX, var posY) = Location;
            //(var dirX, var dirY) = Direction;
            //var oldDirX = dirX;
            //var rotSpeed = 1.57;
            //var moveSpeed = 1.0;

            //if (WalkTimer == 0)
            //{
            //    posX += dirX * moveSpeed;
            //    posY += dirY * moveSpeed;
            //    WalkTimer = 15;
            //}
            //else if (TurnTimer <= 0)
            //{
            //    if (Random.NextDouble() < .5)
            //        rotSpeed = -rotSpeed;

            //    dirX = dirX * -rotSpeed.Cos() - dirY * -rotSpeed.Sin();
            //    dirY = oldDirX * -rotSpeed.Sin() + dirY * -rotSpeed.Cos();

            //    TurnTimer = Random.Next(10, 25);
            //}

            //(posX, posY) = (posX, posY).Round();
            //if (!map.BlockedAt(0, (int)posX, (int)posY))
            //{
            //    Location = (posX, posY);
            //    Direction = (dirX, dirY).Round();
            //}
        }
    }
}
