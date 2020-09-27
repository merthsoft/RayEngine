using RayLib;
using RayLib.Defs;
using RayLib.Extensions;
using RayLib.Objects;
using System.Collections.Generic;

namespace RayEngine.Actors
{
    class SecretWall : Actor<bool>
    {
        protected static readonly State<ActionParameters, bool> waitingState = new(CheckInteraction, false);
        protected static readonly State<ActionParameters, bool> openingState = new(MoveWall, false);
        protected static readonly State<ActionParameters, bool> doneState = new(NoAction, true);
        
        private List<RayTexture> Textures { get; } = new();

        public WallDef BackWall { get; set; } = WallDef.Empty;

        public override bool BlocksView => CurrentState!.Data;

        static SecretWall()
        {
            waitingState
                .Chain(openingState)
                .Chain(waitingState);
        }

        public SecretWall()
           => BackWall = WallDef.Empty;

        public SecretWall(ActorDef def)
            : base(def)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            Textures.Add(BackWall.EastWestTexture);
            Textures.Add(BackWall.NorthSouthTexture);

            CurrentState = waitingState;
        }

        public override RayTexture GetTexture(double viewAngle)
            => Textures[viewAngle.CardinalDirection2IndexDegrees()];

        public static bool CheckInteraction(ActionParameters parameters)
        {
            if (!InteractingWith(parameters))
                return false;

            parameters.Actor.Direction = parameters.Player.Direction;
            return true;
        }

        public static bool MoveWall(ActionParameters parameters)
        {
            if (!TimerUp(parameters))
                return false;

            var secretWall = parameters.Actor;
            secretWall.ResetTimer();
            secretWall.Location += secretWall.Direction * .025;
            
            if (!parameters.Map.BlockedAt(0, secretWall.Location + secretWall.Direction * .025, secretWall))
                return true;
            
            secretWall.Location = ((int)secretWall.Location.X, (int)secretWall.Location.Y);
            return true;
        }
    }
}
