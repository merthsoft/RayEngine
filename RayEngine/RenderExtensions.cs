using Microsoft.Xna.Framework;
using RayLib;
using RayLib.Defs;
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
            {
                if (intersection is WallIntersection wall)
                    screen.RenderWallTexture(wall, viewHeight);
                else if (intersection is ObjectIntersection obj)
                    screen.RenderObjectTexture(obj, viewHeight);
            }
            return screen;
        }

        private static void RenderWallTexture(this IActiveRenderer screen, WallIntersection wall, int viewHeight)
            => RenderTexture(screen, wall, viewHeight, wall.NorthWall ? wall.WallDef.NorthSouthTexture : wall.WallDef.EastWestTexture);

        private static void RenderObjectTexture(this IActiveRenderer screen, ObjectIntersection obj, int viewHeight) 
            => RenderTexture(screen, obj, viewHeight, (obj.Def as StaticObjectDef)!.Texture);

        private static void RenderTexture(IActiveRenderer screen, Intersection intersection, int viewHeight, RayTexture texture)
        {
            var column = texture[intersection.TextureX];
            var step = 1.0 * 64 / intersection.Height;
            var texPos = (intersection.Top - viewHeight / 2 + intersection.Height / 2) * step;
            for (var y = intersection.Top; y < intersection.Bottom; y++)
            {
                var texY = (int)texPos & (63);
                texPos += step;
                (var a, var r, var g, var b) = column[texY];
                var distanceScale = (int)(6 * intersection.Distance);
                screen.PlotPoint(intersection.ScreenX, y, a, r - distanceScale, g - distanceScale, b - distanceScale);
            }
        }
    }
}
