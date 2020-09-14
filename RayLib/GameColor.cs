using System;

namespace RayLib
{
    public record GameColor
    {
        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public bool IsWhiteTransparent => A == 0 && R == 255 && G == 255 && B == 255;

        public void Deconstruct(out int a, out int r, out int g, out int b)
            => (a, r, g, b)
             = (A, R, G, B);

        public GameColor() { }

        public GameColor(GameColor gameColor)
        {
            A = gameColor.A;
            R = gameColor.R;
            G = gameColor.G;
            B = gameColor.B;
        }

        public GameColor(int a, int r, int g, int b)
            => (A, R, G, B)
             = (a, r, g, b);

        public GameColor AlphaClamp(int clamp)
        {
            A = A <= clamp ? 0 : 255;
            return this;
        }
        
        public GameColor TransformColorChannels(Func<int, int> transform)
        {
            R = transform(R);
            G = transform(G);
            B = transform(B);

            return this;
        }

        public GameColor Copy(GameColor gameColor)
        {
            A = gameColor.A;
            R = gameColor.R;
            G = gameColor.G;
            B = gameColor.B;

            return this;
        }
    }
}