using RayLib;
using RayLib.Defs;
using RayLib.Extensions;
using RayLib.Objects;
using System;

namespace RayEngine.Actors
{
    public class FastWanderer : Actor
    {
        protected static readonly State<ActionParameters> WalkState = new(5, 10, Walk);
        protected static readonly State<ActionParameters> TurnState = new(5, Turn);

        static FastWanderer()
        {
            WalkState.AddNext(TurnState);
            TurnState.AddNext(WalkState, WalkState, TurnState);
        }

        public FastWanderer() : base() { }

        public FastWanderer(ActorDef actorDef)
            : base(actorDef) { }

        public override void Initialize()
        {
            base.Initialize();
            CurrentState = WalkState;
        }

        public override void Act(ActionParameters parameters) 
            => base.Act(parameters);

        private static bool Turn(ActionParameters actionParameters)
        {
            if (!TimerUp(actionParameters))
                return false;

            (var dirX, var dirY) = actionParameters.Actor.Direction;
            var oldDirX = dirX;
            var rotSpeed = 1.57;

            if (Random.NextDouble() < .5)
                rotSpeed = -rotSpeed;

            dirX = dirX * -rotSpeed.Cos() - dirY * -rotSpeed.Sin();
            dirY = oldDirX * -rotSpeed.Sin() + dirY * -rotSpeed.Cos();
            actionParameters.Actor.Direction = (dirX, dirY).Round();

            return true;
        }

        private static bool Walk(ActionParameters actionParameters)
        {
            if (!TimerUp(actionParameters))
                return false;

            (var posX, var posY) = actionParameters.Actor.Location;
            (var dirX, var dirY) = actionParameters.Actor.Direction;
            
            posX += dirX * .25;
            posY += dirY * .25;

            (posX, posY) = (posX, posY).Round();
            if (!actionParameters.Map.BlockedAt(0, (int)posX, (int)posY))
                actionParameters.Actor.Location = (posX, posY);

            return true;
        }
    }
}
