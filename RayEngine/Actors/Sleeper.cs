using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System.Linq;

namespace RayEngine.Actors
{
    public class Sleeper : Actor
    {
        protected static readonly State<ActionParameters> LookState = new(50, Look);
        protected static readonly State<ActionParameters> AwakeState = new(50, TimerUp);
        protected static readonly State<ActionParameters> PopUpState = new(50, PopUp);
        protected static readonly State<ActionParameters> FollowState = new(40, 70, Follow);

        static Sleeper()
        {
            LookState
                .Chain(AwakeState)
                .Chain(PopUpState)
                .Chain(FollowState);
        }

        public Sleeper() : base() { }

        public Sleeper(ActorDef actorDef)
            : base(actorDef) { }

        public override void Initialize()
        {
            base.Initialize();
            CurrentState = LookState;
            TextureIndex = 0;
        }

        public static bool Look(ActionParameters actionParameters)
        {
            if (!TimerUp(actionParameters))
                return false;

            if (actionParameters.Map.FindObjectsInSight(actionParameters.Actor, (0, 0)).Contains(actionParameters.Player))
                return true;

            return false;
        }

        public static bool PopUp(ActionParameters actionParameters)
        {
            actionParameters.Actor.TextureIndex = 1;
            if (!(actionParameters.Player is GamePlayer gamePlayer))
                return true;

            gamePlayer.FieldOfView -= .005;

            if (!TimerUp(actionParameters))
                return false;

            return true;
        }

        public static bool Follow(ActionParameters actionParameters)
        {
            actionParameters.Actor.TextureIndex = 2;
            if (!(actionParameters.Player is GamePlayer gamePlayer))
                return true;

            if (gamePlayer.FieldOfView < .75)
                gamePlayer.FieldOfView += .01;

            if (!TimerUp(actionParameters))
                return false;

            gamePlayer.FieldOfView = .75;

            var actor = actionParameters.Actor;
            var map = actionParameters.Map;

            var direction = GameVector.CardinalDirections8[(actor.Location - gamePlayer.Location).Atan2().ToDegrees().CardinalDirection8IndexDegrees()];
            actor.Direction = direction;

            var newLocation = map.FindPath(actor.Location, gamePlayer.Location).FirstOrDefault();
            if (newLocation == null)
                newLocation = actor.Location + direction;

            if (newLocation == gamePlayer.Location)
            {
                gamePlayer.ScreenFlash = (64, 0, 0);
                gamePlayer.ScreenFlashDuration += 50;
                gamePlayer.ScreenFlashDecay = 1;
            }

            if (map.BlockedAt(0, newLocation))
                newLocation = actor.Location - direction;

            if (!map.BlockedAt(0, newLocation))
                actor.Location = newLocation;

            return true;
        }
    }
}
