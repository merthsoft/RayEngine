using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System;

namespace RayEngine.Actors
{
    public class Wanderer : Actor
    {
        private static readonly Random Random = new Random();

        private int WalkTimer = 15;
        private int TurnTimer = 25;

        public Wanderer() : base() { }

        public Wanderer(ActorDef actorDef)
            : base(actorDef) { }

        public override void Act(Map map)
        {
             TurnTimer--;
            WalkTimer--;

            (var posX, var posY) = Location;
            (var dirX, var dirY) = Direction;
            (var planeX, var planeY) = Plane;
            var oldDirX = dirX;
            var rotSpeed = 1.57;
            var moveSpeed = 1.0;

            if (WalkTimer == 0)
            {
                posX += dirX * moveSpeed;
                posY += dirY * moveSpeed;
                WalkTimer = 15;
            }
            else if (TurnTimer <= 0)
            {
                if (Random.NextDouble() < .5)
                    rotSpeed = -rotSpeed;

                dirX = dirX * -rotSpeed.Cos() - dirY * -rotSpeed.Sin();
                dirY = oldDirX * -rotSpeed.Sin() + dirY * -rotSpeed.Cos();
                var oldPlaneX = planeX;
                planeX = planeX * -rotSpeed.Cos() - planeY * -rotSpeed.Sin();
                planeY = oldPlaneX * -rotSpeed.Sin() + planeY * -rotSpeed.Cos();

                TurnTimer = Random.Next(10, 25);
            }

            if (map[0, posX, posY] == WallDef.Empty)
            {
                Location = (posX, posY).Round();
                Direction = (dirX, dirY).Round();
                Plane = (planeX, planeY).Round();
            }
        }
    }
}
