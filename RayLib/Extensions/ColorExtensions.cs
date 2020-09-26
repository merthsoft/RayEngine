namespace RayLib.Extensions
{
    public static class ColorExtensions
    {
        public const uint AlphaMask = 0xFF000000;
        public const uint ColorMask = 0x00FFFFFF;
        public const uint RedMask = 0x00FF0000;
        public const uint GreenMask = 0x0000FF00;
        public const uint BlueMask = 0x000000FF;
        public const uint WhiteMask = 0x00FFFFFF;
        public const uint BlackMask = 0x00000000;

        public const uint White = AlphaMask | WhiteMask;
        public const uint Black = AlphaMask | BlackMask;

        public static bool IsWhiteTransparent(this uint argb)
            => (argb >> 24) < 0xFF && (argb & ColorMask) == WhiteMask;

        public static uint Blend(this uint color1, uint color2)
        {
            var a1 = (color1 & AlphaMask) >> 24;
            var a2 = (color2 & AlphaMask) >> 24;
            var outA = a1 + a2 * (1 - a1);
            var r1 = (color1 & RedMask) >> 24;
            var r2 = (color2 & RedMask) >> 24;
            var g1 = (color1 & GreenMask) >> 24;
            var g2 = (color2 & GreenMask) >> 24;
            var b1 = (color1 & BlueMask) >> 24;
            var b2 = (color2 & BlueMask) >> 24;

            uint blend(uint channel1, uint channel2)
                => BlendChannel(a1, a2, outA, channel1, channel2);

            return (outA << 24) | (blend(r1, r2) << 16) | (blend(g1, g2) << 8) | blend(b1, b2);
        }

        private static uint BlendChannel(uint a1, uint a2, uint outA, uint channel1, uint channel2)
            => (uint)((channel1 * a1 + channel2 * a2 * (1 - a1)) / (double)outA).Round(0);
    }
}
