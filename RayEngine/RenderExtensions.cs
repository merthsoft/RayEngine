using Microsoft.Xna.Framework;
using RayEngine.Actors;
using RayLib;
using System;

namespace RayEngine
{
    static class RenderExtensions
    {
        private static readonly uint CielingColor = new Color(Color.Brown.R - 100, Color.Brown.G, Color.Brown.B).PackedValue;
        private static readonly uint FloorColor = new Color(64, 64, 64).PackedValue;

        public static Renderer RenderWorld(this Renderer screen, int viewWidth, int viewHeight, Step step)
        {
            for (var y = 0; y < viewHeight / 2; y++)
            {
                var colorScale = (double)viewHeight / ((double)viewHeight - (double)y);
                screen
                   .DrawHorizonalLine(0, y, viewWidth, DarkenColor(CielingColor, 1/colorScale))
                   .DrawHorizonalLine(0, y + viewHeight / 2, viewWidth, DarkenColor(FloorColor, colorScale));
            }

            foreach (var intersection in step.Intersections)
                intersection.Render(screen, viewHeight, DarkByDistance);
            return screen;
        }

        private static uint DarkByDistance(int x, int y, double distance, double viewAngleDegrees, uint color)
            => DarkenColor(color, 2.0 / (distance + 2) );

        private static uint DarkenColor(uint color, double ratio)
        {
            ratio = ratio > 1 ? 1 : ratio;
            var a = (color >> 24) & 0xFF;
            var r = ((color >> 16) & 0xFF) * ratio;
            r = r > 0xFF ? 0xFF : r;
            var g = ((color >> 8) & 0xFF) * ratio;
            g = g > 0xFF ? 0xFF : g;
            var b = (color & 0xFF) * ratio;
            b = b > 0xFF ? 0xFF : b;

            return (a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
        }

        public static Renderer RenderScreenFlash(this Renderer screen, int viewWidth, int viewHeight, GamePlayer player)
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
