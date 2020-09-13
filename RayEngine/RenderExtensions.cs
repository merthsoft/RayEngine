using Microsoft.Xna.Framework;
using RayEngine.Actors;
using RayLib;
using System;

namespace RayEngine
{
    public static class RenderExtensions
    {
        public static int Remap(this int value, int from1, int to1, int from2, int to2)
            => (int)((float)(value - from1) / (float)(to1 - from1) * (float)(to2 - from2) + (float)from2);

        public static IActiveRenderer RenderWorld(this IActiveRenderer screen, int viewWidth, int viewHeight, Step step)
        {
            for (var y = 0; y < viewHeight / 2; y++)
            {
                var colorScale = (int)(y / 1.5).Round(0);
                screen
                   .DrawLine(0, y, viewWidth, y, 255, Color.Brown.R - 100 - colorScale, Color.Brown.G - colorScale, Color.Brown.B - colorScale)
                   .DrawLine(0, y + viewHeight / 2, viewWidth, y + viewHeight / 2, 255, 64 + colorScale, 64 + colorScale, 64 + colorScale);
            }

            foreach (var intersection in step.Intersections)
                intersection.Render(screen, viewHeight);

            return screen;
        }

        public static IActiveRenderer RenderScreenFlash(this IActiveRenderer screen, int viewWidth, int viewHeight, GamePlayer player)
        {
            if (player.ScreenFlashDuration == 0)
                return screen;
            (var r, var g, var b) = player.ScreenFlash;
            var a = (int)Math.Min(240, player.ScreenFlashDuration);
            for (var y = 0; y < viewHeight; y++)
                screen
                    .DrawLine(0, y, viewWidth, y, a, r, g, b);

            return screen;
        }
    }
}
