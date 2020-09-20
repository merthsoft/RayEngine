namespace RayLib.Extensions
{
    public static class Color
    {
        public const uint ColorMask = 0x00FFFFFF;
        public const uint TransparentWhite = 0x00FFFFFF;
        public const uint TransparentBlack = 0x00000000;
        public const uint White = 0xFFFFFFFF;
        public const uint Black = 0xFF000000;

        public static bool IsWhiteTransparent(this uint argb)
            => (argb >> 24) < 0xFF && (argb & ColorMask) == TransparentWhite;
    }
}
