using RayLib.Defs;
using System;

namespace RayLib.Intersections
{
    public abstract record Intersection
    {
        public Def Def { get; }
        public int ScreenX { get; }
        public int TextureX { get; }
        public int Top { get; }
        public int Bottom { get; }
        public double Distance { get; }
        public int Height { get; }
        public double ViewAngleDegrees { get; set; }

        protected Intersection(Def def, int screenX, int textureX, int top, int bottom, double distance, int height, double viewAngleDegrees)
            => (Def, ScreenX, TextureX, Top, Bottom, Distance, Height, ViewAngleDegrees)
             = (def, screenX, textureX, top, bottom, distance, height, viewAngleDegrees);

        public abstract RayTexture GetTexture();

        public IActiveRenderer Render(IActiveRenderer screen, int viewHeight)
        {
            var texture = GetTexture();
            var column = texture[TextureX];
            var step = (double)Def.DrawSize.H / Height;
            var texPos = (Top - viewHeight / 2.0 + Height / 2.0) * step;

            var textureHeight = Def.DrawSize.H - 1;
            var distanceScale = (int)(13 * Distance); // TODO: This should be in the engine, not the lib
            for (var y = Top; y < Bottom; y++)
            {
                var texY = (int)texPos & textureHeight;
                texPos += step;
                var c = column[texY];
                if (c.A != 255)
                    continue;
                screen.PlotPoint(ScreenX, y, 255, c.R - distanceScale, c.G - distanceScale, c.B - distanceScale);
            }

            return screen;
        }
    }
}
