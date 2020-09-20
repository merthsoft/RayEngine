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
    }
}
