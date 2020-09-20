using Microsoft.Xna.Framework;
using RayEngine.Actors;
using RayLib;
using System;

namespace RayEngine
{
    public static class RenderExtensions
    {
        private static readonly uint CielingColor = new Color(Color.Brown.R - 100, Color.Brown.G, Color.Brown.B).PackedValue;
        private static readonly uint FloorColor = new Color(64, 64, 64).PackedValue;

        public static IActiveRenderer RenderWorld(this IActiveRenderer screen, int viewWidth, int viewHeight, Step step)
        {
            for (var y = 0; y < viewHeight / 2; y++)
            {
                var colorScale = (double)viewHeight / ((double)viewHeight - (double)y);
                screen
                   .DrawHorizonalLine(0, y, viewWidth, DarkenColor(CielingColor, 1/colorScale))
                   .DrawHorizonalLine(0, y + viewHeight / 2, viewWidth, DarkenColor(FloorColor, colorScale));
            }

            foreach (var intersection in step.Intersections)
                intersection.Render(screen, viewHeight);

            return screen;
        }

        private static uint DarkenColor(uint color, double ratio)
        {
            var a = (color >> 24) & 0xFF;
            var r = ((color >> 16) & 0xFF) * ratio;
            var g = ((color >> 8) & 0xFF) * ratio;
            var b = (color & 0xFF) * ratio;

            return (a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
        }

        public static IActiveRenderer RenderScreenFlash(this IActiveRenderer screen, int viewWidth, int viewHeight, GamePlayer player)
        {
            if (player.ScreenFlashDuration == 0)
                return screen;
            (var r, var g, var b) = player.ScreenFlash;
            var a = Math.Min(240, player.ScreenFlashDuration);
            var c = (uint)(a << 24 | r << 16 | g << 8 | b);
            for (var y = 0; y < viewHeight; y++)
                screen.DrawHorizonalLine(0, y, viewWidth, c);

            return screen;
        }
    }
}
