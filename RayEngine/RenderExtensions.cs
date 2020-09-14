using Microsoft.Xna.Framework;
using RayEngine.Actors;
using RayLib;
using System;

namespace RayEngine
{
    public static class RenderExtensions
    {
        public static IActiveRenderer RenderWorld(this IActiveRenderer screen, int viewWidth, int viewHeight, Step step)
        {
            for (var y = 0; y < viewHeight / 2; y++)
            {
                var colorScale = y/2;
                screen
                   .DrawLine(0, y, viewWidth, y, 255, Color.Brown.R - 100 - colorScale, Color.Brown.G - colorScale, Color.Brown.B - colorScale)
                   .DrawLine(0, y + viewHeight / 2, viewWidth, y + viewHeight / 2, 255, 64 + colorScale, 64 + colorScale, 64 + colorScale);
            }

            foreach (var intersection in step.Intersections)
                intersection.Render(screen, viewHeight,
                    (int x, int y, double distance, double viewAngleDegrees, ref GameColor color) => 
                    {
                        color.A = color.A < 200 ? 0 : 255;
                        color.R -= (int)(distance * 20);
                        color.G -= (int)(distance * 20);
                        color.B -= (int)(distance * 20);
                    }
                );

            return screen;
        }

        public static IActiveRenderer RenderScreenFlash(this IActiveRenderer screen, int viewWidth, int viewHeight, GamePlayer player)
        {
            if (player.ScreenFlashDuration == 0)
                return screen;
            (var r, var g, var b) = player.ScreenFlash;
            var a = Math.Min(240, player.ScreenFlashDuration);
            for (var y = 0; y < viewHeight; y++)
                screen.DrawLine(0, y, viewWidth, y, a, r, g, b);

            return screen;
        }
    }
}
