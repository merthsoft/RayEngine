using Microsoft.Xna.Framework;
using RayLib;
using System.Linq;

namespace RayEngine
{
    public static class RenderExtensions
    {
        public static int Remap(this int value, int from1, int to1, int from2, int to2)
            => (int)((float)(value - from1) / (float)(to1 - from1) * (float)(to2 - from2) + (float)from2);

        public static IActiveRenderer RenderWorld(this IActiveRenderer screen, int viewWidth, int viewHeight, Step step)
        {
            for (var y = 0; y < viewHeight / 2; y++)
                screen
                    .DrawLine(0, y, viewWidth, y, 255, Color.Brown.R - 100 - y/3, Color.Brown.G - y/3, Color.Brown.B - y/3)
                    .DrawLine(0, y + viewHeight / 2, viewWidth, y + viewHeight / 2, 255, 64 + y/3, 64 + y/3, 64 + y/3);

            foreach (var intersection in step.Intersections)
                intersection.Render(screen, viewHeight);

            return screen;
        }
    }
}
