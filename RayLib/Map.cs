using RayLib.Defs;
using RayLib.Intersections;
using RayLib.Objects;
using System;
using System.Collections.Generic;
using System.IO;
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
        public (int w, int h) ViewSize { get; set; }
        public int NumLayers => Walls.Count;

        public Map(int viewWidth, int viewHeight, string simpleMap, Action<Map, int, int, char, char[,]> generator)
        {
            var splitMap = simpleMap.Split("\n").Select(s => s.Trim().ToArray()).Reverse().ToArray();
            SetSizes(splitMap[0].Length, splitMap.Length, 1, viewWidth, viewHeight);

            var neighbors = new[,] { { '\0', '\0', '\0' }, { '\0', '\0', '\0' }, { '\0', '\0', '\0' } };
            void ExtractNeighbors(char[][] splitMap, int x, int y)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        var entry = ' ';
                        var neighborX = i + x;
                        var neighborY = j + y;
                        if (neighborX >= 0 && neighborY >= 0 && neighborX < Size.w && neighborY < Size.h)
                            entry = splitMap[neighborY][neighborX];
                        neighbors[i + 1, j + 1] = entry;
                    }
                }
            }

            var y = 0;
            foreach (var line in splitMap)
            {
                var x = 0;
                foreach (var c in line)
                {
                    ExtractNeighbors(splitMap, x, y);
                    generator(this, x, y, c, neighbors);
                    x++;
                }
                y++;
            }
        }

        public Map(int width, int height, int layers, int viewWidth, int viewHeight)
            => SetSizes(width, height, layers, viewWidth, viewHeight);

        private void SetSizes(int width, int height, int layers, int viewWidth, int viewHeight)
        {
            Size = (width, height);
            ViewSize = (viewWidth, viewHeight);
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

        private void AddToObjectMap(int layer, GameObject obj)
        {
            var spot = ObjectMap[layer][(int)obj.Location.X][(int)obj.Location.Y];
            spot.Add(obj);
        }

        private void RemoveFromObjectMap(int layer, GameObject obj) 
        {
            var spot = ObjectMap[layer][(int)obj.Location.X][(int)obj.Location.Y];
            spot.Remove(obj);
        }

        public IEnumerable<GameObject> ObjectsInSight(GameObject baseObject, GameVector viewOffset)
        {
            // TODO: Largely taken from Update. Merge.
            var (viewWidth, _) = ViewSize;
            var (posX, posY) = baseObject.Location - viewOffset;

            var screenX = 0;
            for (var x = viewWidth - 1; x >= 0; x--)
            {
                var (mapX, mapY) = baseObject.Location.Floor();
                var cameraX = 2.0 * x / viewWidth - 1.0;
                var rayDir = baseObject.Direction + baseObject.FieldOfView * cameraX;
                var (deltaDistX, deltaDistY) = (1 / rayDir).Abs();

                (var stepX, var sideDistX) = rayDir.X < 0
                    ? (-1, (posX - mapX) * deltaDistX)
                    : (1, (mapX + 1.0 - posX) * deltaDistX);

                (var stepY, var sideDistY) = rayDir.Y < 0
                    ? (-1, (posY - mapY) * deltaDistY)
                    : (1, (mapY + 1.0 - posY) * deltaDistY);

                var wall = WallDef.Empty;
                var blocked = false;
                while (wall == WallDef.Empty && !blocked)
                {
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                    }

                    if (mapX < 0 || mapY < 0 || mapX > Size.w || mapY > Size.h)
                        break;

                    wall = Walls[0][(int)mapX][(int)mapY];
                    var lineObjects = ObjectMap[0][(int)mapX][(int)mapY];
                    foreach (var obj in lineObjects)
                    {
                        yield return obj;
                        if (obj.BlocksView)
                            blocked = true;
                    }
                }

                if (wall == WallDef.Empty)
                    break;

                screenX++;
            }
        }

        public Step Update(Player player, GameVector viewOffset)
        {
            Actors.ForEach(a =>
            {
                RemoveFromObjectMap(0, a);
                a.Act(this, player);
                AddToObjectMap(0, a);
            });

            var (viewWidth, viewHeight) = ViewSize;

            var intersections = new HashSet<Intersection>();
            var zbuffer = new double[viewWidth];
            
            var objectsInSight = new HashSet<GameObject>();
            var directionalObjects = new Dictionary<GameObject, List<Intersection>>();

            var (playerX, playerY) = player.Location - viewOffset;
            var (planeX, planeY) = player.Plane;
            var (dirX, dirY) = player.Direction;

            double distance;
            int lineHeight;
            int drawStart;
            int texX;

            var screenX = 0;
            for (var x = viewWidth - 1; x >= 0; x--)
            {
                var (mapX, mapY) = player.Location.Floor();
                var cameraX = 2.0 * x / viewWidth - 1.0;
                var rayDir = player.Direction + player.Plane * cameraX;
                var (deltaDistX, deltaDistY) = (1 / rayDir).Abs();
                var northWall = false;

                (var stepX, var sideDistX) = rayDir.X < 0
                    ? (-1, (playerX - mapX) * deltaDistX)
                    : (1, (mapX + 1.0 - playerX) * deltaDistX);

                (var stepY, var sideDistY) = rayDir.Y < 0
                    ? (-1, (playerY - mapY) * deltaDistY)
                    : (1, (mapY + 1.0 - playerY) * deltaDistY);

                var wall = WallDef.Empty;
                var blocked = false;
                while (!blocked)
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

                    if (mapX < 0 || mapY < 0 || mapX >= Size.w || mapY >= Size.h)
                        break;

                    wall = Walls[0][(int)mapX][(int)mapY];
                    if (wall != WallDef.Empty)
                        break;

                    var lineObjects = ObjectMap[0][(int)mapX][(int)mapY];
                    foreach (var obj in lineObjects)
                    {
                        if (obj.BlocksView)
                            blocked = true;

                        objectsInSight.Add(obj);
                        
                        if (obj.Direction == GameVector.Zero)
                            continue;
                        
                        if (!directionalObjects.TryGetValue(obj, out var objectIntersections))
                            objectIntersections = new List<Intersection>();
                        directionalObjects[obj] = objectIntersections;
                        (distance, lineHeight, drawStart, texX) = measureWallShit(viewHeight, playerX, playerY, mapX, mapY, rayDir, northWall, stepX, stepY, obj.Def);
                        objectIntersections.Add(new ObjectIntersection(obj, screenX, texX, drawStart, drawStart + lineHeight, distance, lineHeight, (player.Location - (obj.Location.X, obj.Location.Y)).Atan2().ToDegrees()));
                    }
                }

                if (wall != WallDef.Empty)
                {
                    (distance, lineHeight, drawStart, texX) = measureWallShit(viewHeight, playerX, playerY, mapX, mapY, rayDir, northWall, stepX, stepY, wall);
                    zbuffer[screenX] = distance;
                    if (!blocked)
                        intersections.Add(new WallIntersection(wall, screenX, texX, drawStart, drawStart + lineHeight, distance, lineHeight, ((mapX, mapY) - player.Location).Atan2().ToDegrees(), northWall));
                }
                screenX++;
            }

            (playerX, playerY) = player.Location - viewOffset;
            foreach (var obj in objectsInSight.OrderByDescending(o => o.Location.UnscaledDistance(player.Location)))
            {
                if (obj.Direction != GameVector.Zero)
                {
                    var objectIntersections = directionalObjects.GetValueOrDefault(obj);
                    if (objectIntersections == null)
                        continue;
                    foreach (var intersection in objectIntersections)
                        intersections.Add(intersection);

                    continue;
                }

                var locationDelta = (obj.Location - (playerX, playerY)) ;
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
                var spriteWidth = (int)(viewHeight / transformY / (planeX * dirY - dirX * planeY)*.65).Abs();
                var drawStartX = (-spriteWidth / 2 + spriteScreenX);
                if (drawStartX < 0)
                    drawStartX = 0;
                var drawEndX = (spriteWidth / 2 + spriteScreenX);
                if (drawEndX >= viewWidth)
                    drawEndX = viewWidth - 1;

                for (var stripe = (int)drawStartX; stripe < (int)drawEndX; stripe++)
                {
                    screenX = viewWidth - stripe - 1;
                    var textureWidth = (int)obj.Def.DrawSize.W;
                    texX = textureWidth - (stripe - (-spriteWidth / 2 + spriteScreenX)) * textureWidth / spriteWidth - 1;
                    if (transformY > 0 && screenX > 0 && screenX < viewWidth && transformY < zbuffer[screenX])
                        intersections.Add(new ObjectIntersection(obj, screenX, texX, drawStartY, drawEndY, transformY, spriteHeight, angle));
                }
                    
            }

            return new Step(intersections, objectsInSight, new[] { zbuffer });
        }

        private (double distance, int lineHeight, int drawStart, int texX) measureWallShit(int viewHeight, double posX, double posY, double mapX, double mapY, GameVector rayDir, bool northWall, int stepX, int stepY, Def def)
        {
            double distance;
            int lineHeight;
            int drawStart;
            int texX;
            distance = !northWall
                        ? (mapX - posX + (1.0 - stepX) / 2.0) / rayDir.X
                        : (mapY - posY + (1.0 - stepY) / 2.0) / rayDir.Y;
            lineHeight = (int)(viewHeight / distance);
            drawStart = (int)(-lineHeight / 2.0 + viewHeight / 2.0);
            var wallX = !northWall
                ? posY + distance * rayDir.Y
                : posX + distance * rayDir.X;
            wallX -= (int)wallX;

            var textureWidth = def.DrawSize.W;
            texX = (int)(wallX * textureWidth);
            if (!northWall && rayDir.X > 0)
                texX = textureWidth - texX - 1;
            if (northWall && rayDir.Y < 0)
                texX = textureWidth - texX - 1;

            return (distance, lineHeight, drawStart, texX);
        }

        public void SetWall(int layer, int x, int y, WallDef wall)
            => Walls[layer][x][y] = wall;

        public GameObject SpawnObject(int layer, int x, int y, StaticObjectDef def, GameVector? direction = null)
        {
            var obj = new StaticObject(def, x, y);
            if (direction != null)
                obj.Direction = direction;
            obj.Initialize();
            StaticObjects.Add(obj);
            AddToObjectMap(layer, obj);
            return obj;
        }

        public T SpawnActor<T>(int layer, int x, int y, ActorDef def, GameVector? direction = null, double fov = .75, Action<T>? preInit = null) where T : Actor, new()
        {
            var obj = new T() { Def = def , Location = (x + .5, y + .5), Direction = direction ?? GameVector.Zero, FieldOfView = fov };
            preInit?.Invoke(obj);
            obj.Initialize();
            Actors.Add(obj);
            AddToObjectMap(layer, obj);
            return obj;
        }

        public void ClearPlayer(Player player)
            => RemoveFromObjectMap(0, player);

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
            => AddToObjectMap(0, player);

        public IEnumerable<GameVector> FindPath(GameVector pos1, GameVector pos2)
            => AStar.Search(pos1, pos2, this, (p1, p2) => p1.UnscaledDistance(p2));
        
        public IEnumerable<GameVector> GetNeighbors(GameVector root)
        {
            foreach (var direction in GameVector.CardinalDirections8.Select(v => v.Round(0)))
            {
                var n = root + direction;
                if (!BlockedAt(0, n, false))
                    yield return n;
            }
        }
    }
}
