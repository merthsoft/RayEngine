using RayLib.Defs;

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
        public double Angle { get; set; }

        protected Intersection(Def def, int screenX, int textureX, int top, int bottom, double distance, int height, double angle)
            => (Def, ScreenX, TextureX, Top, Bottom, Distance, Height, Angle)
             = (def, screenX, textureX, top, bottom, distance, height, angle);

        public abstract RayTexture GetTexture();

        public IActiveRenderer Render(IActiveRenderer screen, int viewHeight)
        {
            var texture = GetTexture();
            var column = texture[TextureX];
            var step = (double)Def.DrawSize.H / (double)Height;
            var texPos = (Top - viewHeight / 2 + Height / 2) * step;

            var textureHeight = (int)Def.DrawSize.H - 1;
            var distanceScale = (int)(6 * Distance);
            for (var y = Top; y < Bottom; y++)
            {
                var texY = (int)texPos & textureHeight;
                texPos += step;
                (var a, var r, var g, var b) = column[texY];
                screen.PlotPoint(ScreenX, y, a, r - distanceScale, g - distanceScale, b - distanceScale);
            }

            return screen;
        }
    }
}
