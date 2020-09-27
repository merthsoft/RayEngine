using RayLib;
using RayLib.Objects;

namespace RayEngine.Actors
{
    class PushableObject : Actor
    {
        protected static readonly State<ActionParameters> waitingState = new(CheckInteraction);
        protected static readonly State<ActionParameters> movingState = new(40, PushObject);

        static PushableObject()
        {
            waitingState
                .Chain(movingState)
                .Chain(waitingState);
        }

        public override void Initialize()
        {
            base.Initialize();

            CurrentState = waitingState;
        }

        public static bool CheckInteraction(ActionParameters parameters)
        {
            if (!InteractingWith(parameters))
                return false;

            parameters.Actor.Direction = parameters.Player.Direction;
            return true;
        }

        public static bool PushObject(ActionParameters parameters)
        {
            var actor = parameters.Actor;
            actor.Location += actor.Direction * .025;

            if (parameters.Map.BlockedAt(0, actor.Location + actor.Direction * .025, actor) || TimerUp(parameters))
            {
                actor.Location = actor.Location.Floor() + .5;
                return true;
            }

            return false;
        }
    }
}
