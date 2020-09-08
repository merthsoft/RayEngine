using RayLib.Defs;
using RayLib.Intersections;
using RayLib.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RayLib
{
    public class Map : INeighborable<GameVector>
    {
        private List<List<List<WallDef>>> Walls { get; } = new();
        private List<List<List<List<GameObject>>>> ObjectMap { get; } = new();
        private List<StaticObject> StaticObjects { get; } = new();
        private List<Actor> Actors { get; } = new();

        public (int w, int h) Size { get; private set; }
        public int NumLayers => Walls.Count;

        public Map(int width, int height, string simpleMap, Action<Map, int, int, char> generator)
            : this(width, height, 1)
        {
            var i = 0;
            foreach (var line in simpleMap.Split('\n').Select(s => s.Trim()))
            {
                var j = 0;
                foreach (var c in line)
                {
                    generator(this, i, j, c);
                    j++;
                }
                i++;
            }
        }
        
        public Map(int width, int height, int layers)
        {
            Size = (width, height);
            for (var l = 0; l < layers; l++)
            {
                var wallLayer = new List<List<WallDef>>();
                var objectLayer = new List<List<List<GameObject>>>();
                for (var x = 0; x < width; x++)
                {
                    var wallSlice = new List<WallDef>();
                    var objectSlice = new List<List<GameObject>>();
                    for (var y = 0; y < height; y++)
                    {
                        wallSlice.Add(WallDef.Empty);
                        objectSlice.Add(new List<GameObject>());
                    }
                    wallLayer.Add(wallSlice);
                    objectLayer.Add(objectSlice);
                }
                Walls.Add(wallLayer);
                ObjectMap.Add(objectLayer);
            }
        }

        private void AddToObjectMap(GameObject obj)
        {
            var spot = ObjectMap[0][(int)obj.Location.X][(int)obj.Location.Y];
            spot.Add(obj);
        }

        private void RemoveFromObjectMap(GameObject obj) 
        {
            var spot = ObjectMap[0][(int)obj.Location.X][(int)obj.Location.Y];
            spot.Remove(obj);
        }

        public Step Update(Player player, int viewWidth, int viewHeight, GameVector viewOffset)
        {
            Actors.ForEach(a =>
            {
                RemoveFromObjectMap(a);
                a.Act(this, player);
                AddToObjectMap(a);
            });

            var intersections = new List<Intersection>();
            var zbuffer = new double[viewWidth];
            var objectsInSight = new HashSet<GameObject>();

            var (posX, posY) = player.Location - viewOffset;
            var (planeX, planeY) = player.Plane;
            var (dirX, dirY) = player.Direction;

            var screenX = 0;
            for (var x = viewWidth - 1; x >= 0; x--)
            {
                var (mapX, mapY) = player.Location.Floor();
                var cameraX = 2.0 * x / viewWidth - 1.0;
                var rayDir = player.Direction + player.Plane * cameraX;
                var (deltaDistX, deltaDistY) = (1 / rayDir).Abs();
                var eastWall = false;

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
                        eastWall = false;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        eastWall = true;
                    }

                    if (mapX < 0 || mapY < 0 || mapX > Size.w || mapY > Size.h)
                        break;

                    wall = Walls[0][(int)mapX][(int)mapY];
                    var lineObjects = ObjectMap[0][(int)mapX][(int)mapY];
                    foreach (var obj in lineObjects)
                        objectsInSight.Add(obj);
                }

                if (wall == WallDef.Empty)
                    break;

                var distance = !eastWall
                            ? (mapX - posX + (1.0 - stepX) / 2.0) / rayDir.X
                            : (mapY - posY + (1.0 - stepY) / 2.0) / rayDir.Y;

                var lineHeight = (int)(viewHeight / distance);

                var drawStart = (int)(-lineHeight / 2.0 + viewHeight / 2.0);

                var wallX = !eastWall
                    ? posY + distance * rayDir.Y
                    : posX + distance * rayDir.X;
                wallX -= (int)wallX;

                var textureWidth = wall.DrawSize.W;
                var texX = (int)(wallX * textureWidth);
                if (!eastWall && rayDir.X > 0)
                    texX = textureWidth - texX - 1;
                if (eastWall && rayDir.Y < 0)
                    texX = textureWidth - texX - 1;

                zbuffer[screenX] = distance;
                intersections.Add(new WallIntersection(wall, screenX, texX, drawStart, drawStart + lineHeight, distance, lineHeight, ((mapX, mapY) - player.Location).Atan2(), !eastWall));
                screenX++;
            }

            foreach (var obj in objectsInSight.OrderByDescending(o => o.Location.UnscaledDistance(player.Location)))
            {
                var locationDelta = obj.Location - (posX, posY);
                var angle = locationDelta.Atan2().ToDegrees();
                var (spriteX, spriteY) = locationDelta - viewOffset * .7;
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
                
                for (var stripe = drawStartX; stripe < drawEndX; stripe++)
                {
                    screenX = viewWidth - stripe - 1;
                    var textureWidth = (int)obj.Def.DrawSize.W;
                    var texX = textureWidth - (stripe - (-spriteWidth / 2 + spriteScreenX)) * textureWidth / spriteWidth - 1;
                    if (transformY > 0 && screenX > 0 && screenX < viewWidth && transformY < zbuffer[screenX])
                        intersections.Add(new ObjectIntersection(obj, screenX, texX, drawStartY, drawEndY, transformY, spriteHeight, angle));
                }
            }

            return new Step(intersections, objectsInSight, new[] { zbuffer });
        }

        public void SetWall(int layer, int x, int y, WallDef wall)
            => Walls[layer][x][y] = wall;

        public GameObject SpawnObject(int layer, int x, int y, StaticObjectDef def)
        {
            var obj = new StaticObject(def, x, y);
            StaticObjects.Add(obj);
            AddToObjectMap(obj);
            return obj;
        }

        public GameObject SpawnActor<T>(int layer, int x, int y, ActorDef def) where T : Actor, new()
        {
            var w = new T() { Def = def , Location = (x + .5, y + .5) };
            Actors.Add(w);
            AddToObjectMap(w);
            return w;
        }

        public void ClearPlayer(Player player)
            => RemoveFromObjectMap(player);

        public bool BlockedAt(int layer, GameVector location, bool playerBlocks = true)
            => BlockedAt(layer, (int)location.X, (int)location.Y, playerBlocks);

        public bool BlockedAt(int layer, int x, int y, bool playerBlocks = true)
        {
            if (x < 0 || y < 0 || x >= Size.w || y >= Size.h)
                return true;
            if (Walls[layer][x][y] != WallDef.Empty)
                return true;
            return ObjectMap[layer][x][y].Any(o => o is Player p ? o.Blocking && playerBlocks : o.Blocking);
        }

        public void SetPlayer(Player player)
            => AddToObjectMap(player);

        public IEnumerable<GameVector> FindPath(GameVector pos1, GameVector pos2)
            => AStar.Search(pos1, pos2, this, (p1, p2) => p1.UnscaledDistance(p2));
        
        public IEnumerable<GameVector> GetNeighbors(GameVector root)
        {
            foreach (var direction in GameVector.CardinalDirections8)
            {
                var n = root + direction;
                if (!BlockedAt(0, n, false))
                    yield return n;
            }
        }
    }
}
