using Microsoft.Xna.Framework;
using RayLib;
using RayLib.Intersections;
using System.Collections.Generic;

namespace RayEngine
{
    public static class RenderExtensions
    {
        public static int Remap(this int value, int from1, int to1, int from2, int to2)
            => (int)((float)(value - from1) / (float)(to1 - from1) * (float)(to2 - from2) + (float)from2);

        public static IActiveRenderer RenderWorld(this IActiveRenderer screen, int viewWidth, int viewHeight, List<Intersection> intersections)
        {
            for (var y = 0; y < viewHeight / 2; y++)
                for (var x = 0; x < viewWidth; x++)
                    screen
                        .PlotPoint(x, y, 255, Color.Brown.R - 100 - y/3, Color.Brown.G - y/3, Color.Brown.B - y/3)
                        .PlotPoint(x, y + viewHeight / 2, 255, 64 + y/3, 64 + y/3, 64 + y/3);

            foreach (var intersection in intersections)
                intersection.Render(screen, viewHeight);
            return screen;
        }
    }
}
