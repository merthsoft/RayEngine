using RayLib.Defs;
using System;

namespace RayLib.Objects
{
    public class Player : GameObject
    {
        public override bool Blocking => true;

        public (int r, int g, int b) ScreenFlash { get; set; } = (0, 0, 0);
        public int ScreenFlashDuration { get; set; } = 0;
        public int ScreenFlashDecay { get; set; }

        public Player(Def def) : base(def)
        {

        }

        public override RayTexture GetTexture(double viewAngle)
            => RayTexture.Empty;

        public void Update()
        {
            if (ScreenFlashDuration > 250)
                ScreenFlashDuration = 250;
            if (ScreenFlashDuration > 0)
                ScreenFlashDuration -= Math.Max(1, ScreenFlashDecay);
        }
    }
}
