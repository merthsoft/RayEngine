using RayLib.Defs;
using RayLib.Intersections;
using RayLib.Objects;
using System.Collections.Generic;
using System.Linq;

namespace RayLib
{
    public class Map
    {
        private List<WallDef[,]> Layers { get; } = new();
        private List<StaticObject> StaticObjects { get; } = new();
        private List<Actor> Actors { get; } = new();

        public (int w, int h) Size { get; private set; }
        public int NumLayers => Layers.Count;

        public WallDef this[int l, double x, double y]
        {
            get => l < 0 || l >= NumLayers
                || x < 0 || x >= Size.w
                || y < 0 || y >= Size.h
                ? WallDef.Empty
                : Layers[l][(int)x, (int)y];
            set => Layers[l][(int)x, (int)y] = value;
        }

        public Map(int width, int height, int layers)
        {
            Size = (width, height);
            for (var l = 0; l < layers; l++)
            {
                var arr = new WallDef[width, height];
                for (var x = 0; x < width; x++)
                    for (var y = 0; y < height; y++)
                        arr[x, y] = WallDef.Empty;
                Layers.Add(arr);
            }
        }

        private List<GameObject>?[,] GenerateObjectMap()
        {
            var ret = new List<GameObject>[Size.w, Size.h];

            foreach (var obj in StaticObjects.Union<GameObject>(Actors))
            {
                var spot = ret[(int)obj.Location.X, (int)obj.Location.Y];
                if (spot == null)
                    spot = ret[(int)obj.Location.X, (int)obj.Location.Y] = new List<GameObject>();
                spot.Add(obj);
            }

            return ret;
        }

        public Step Update(GameObject requestingObject, int viewWidth, int viewHeight)
        {
            Actors.ForEach(a => a.Act(this));

            var intersections = new List<Intersection>();
            var zbuffer = new double[viewWidth];
            var objectsInSight = new HashSet<GameObject>();
            var objectMap = GenerateObjectMap();


            for (var x = 0; x < viewWidth; x++)
            {
                var (posX, posY) = requestingObject.Location;
                var (mapX, mapY) = requestingObject.Location.Floor();
                var cameraX = 2.0 * x / viewWidth - 1.0;
                var rayDir = requestingObject.Direction + requestingObject.Plane * cameraX;
                var (deltaDistX, deltaDistY) = (1 / rayDir).Abs();
                var northWall = false;

                (var stepX, var sideDistX) = rayDir.X < 0
                    ? (-1, (posX - mapX) * deltaDistX)
                    : (1, (mapX + 1.0 - posX) * deltaDistX);

                (var stepY, var sideDistY) = rayDir.Y < 0
                    ? (-1, (posY - mapY) * deltaDistY)
                    : (1, (mapY + 1.0 - posY) * deltaDistY);

                var wall = WallDef.Empty;
                while (wall == WallDef.Empty)
                {
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        northWall = false;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        northWall = true;
                    }

                    wall = this[0, mapX, mapY];
                    var lineObjects = objectMap[(int)mapX, (int)mapY];
                    if (lineObjects != null)
                        foreach (var obj in lineObjects)
                            objectsInSight.Add(obj);
                }

                var distance = !northWall
                            ? (mapX - posX + (1.0 - stepX) / 2.0) / rayDir.X
                            : (mapY - posY + (1.0 - stepY) / 2.0) / rayDir.Y;

                var lineHeight = (int)(viewHeight / distance);

                var drawStart = (int)(-lineHeight / 2.0 + viewHeight / 2.0);

                var wallX = !northWall
                    ? posY + distance * rayDir.Y
                    : posX + distance * rayDir.X;
                wallX -= (int)wallX;

                var textureWidth = wall.DrawSize.W;
                var texX = (int)(wallX * textureWidth);
                if (!northWall && rayDir.X > 0)
                    texX = textureWidth - texX - 1;
                if (northWall && rayDir.Y < 0)
                    texX = textureWidth - texX - 1;

                zbuffer[x] = distance;
                intersections.Add(new WallIntersection(wall, x, texX, drawStart, drawStart + lineHeight, distance, lineHeight, ((mapX, mapY) - requestingObject.Location).Atan2()));
            }

            foreach (var obj in objectsInSight.OrderByDescending(o => o.Location.UnscaledDistance(requestingObject.Location)))
            {
                var (planeX, planeY) = requestingObject.Plane;
                var (dirX, dirY) = requestingObject.Direction;
                var locationDelta = obj.Location - requestingObject.Location;
                var (spriteX, spriteY) = locationDelta - requestingObject.Direction * .2;
                var invDet = 1.0 / (planeX * dirY - dirX * planeY);

                var transformX = invDet * (dirY * spriteX - dirX * spriteY);
                var transformY = invDet * (-planeY * spriteX + planeX * spriteY);

                var spriteScreenX = (int)((viewWidth / 2) * (1 + transformX / transformY));

                var spriteHeight = (int)(viewHeight / transformY).Abs();

                var drawStartY = -spriteHeight / 2 + viewHeight / 2;
                if (drawStartY < 0)
                    drawStartY = 0;
                var drawEndY = spriteHeight / 2 + viewHeight / 2;
                if (drawEndY >= viewHeight)
                    drawEndY = viewHeight - 1;

                //calculate width of the sprite
                var spriteWidth = (int)(viewHeight / transformY).Abs();
                var drawStartX = -spriteWidth / 2 + spriteScreenX;
                if (drawStartX < 0)
                    drawStartX = 0;
                var drawEndX = spriteWidth / 2 + spriteScreenX;
                if (drawEndX >= viewWidth)
                    drawEndX = viewWidth - 1;

                var angle = locationDelta.Atan2();
                for (int stripe = drawStartX; stripe < drawEndX; stripe++)
                {
                    var textureWidth = (int)obj.Def.DrawSize.W;
                    var texX = (stripe - (-spriteWidth / 2 + spriteScreenX)) * textureWidth / spriteWidth;
                    if (transformY > 0 && stripe > 0 && stripe < viewWidth && transformY < zbuffer[stripe])
                    {
                        intersections.Add(new ObjectIntersection(obj, stripe, texX, drawStartY, drawEndY, transformY, spriteHeight, angle));
                    }
                }
            }

            return new Step(intersections, objectMap, objectsInSight, new[] { zbuffer });
        }

        public GameObject SpawnObject(int x, int y, StaticObjectDef def)
        {
            var obj = new StaticObject(def, x, y);
            StaticObjects.Add(obj);
            return obj;
        }

        public GameObject SpawnActor<T>(int x, int y, ActorDef def) where T : Actor, new()
        {
            var w = new T() { Def = def , Location = (x + .5, y + .5) };
            Actors.Add(w);
            return w;
        }
    }
}
