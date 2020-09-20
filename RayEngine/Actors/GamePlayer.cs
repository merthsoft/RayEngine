using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System;

namespace RayEngine.Actors
{
    public class GamePlayer : Player
    {
        public enum FaceState { Forward, Left, Right, Surprised };

        public (int r, int g, int b) ScreenFlash { get; set; } = (0, 0, 0);
        public int ScreenFlashDuration { get; set; }
        public int ScreenFlashDecay { get; set; }

        public FaceState Face { get; set; }
        public int FaceTimer { get; set; } = 40;

        public bool NoClip { get; set; }

        public GamePlayer(ActorDef def) : base(def)
        {

        }

        public override void Act(ActionParameters parameters)
        {
            if (ScreenFlashDuration > 250)
            {
                ScreenFlashDuration = 250;
                Face = FaceState.Surprised;
                FaceTimer = 15;
            }

            if (ScreenFlashDuration > 0)
                ScreenFlashDuration -= Math.Max(1, ScreenFlashDecay);

            FaceTimer--;
            if (FaceTimer == 0)
            {
                (Face, FaceTimer) = Face switch
                {
                    FaceState.Surprised => (FaceState.Forward, Random.Next(30, 40)),
                    _ => ((FaceState)Random.Next(3), Random.Next(30, 40)),
                };
            }
        }
    }
}
