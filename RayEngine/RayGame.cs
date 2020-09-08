using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.VectorDraw;
using RayEngine.Actors;
using RayLib;
using RayLib.Defs;
using RayLib.Objects;
using System;
using System.Collections.Generic;

namespace RayEngine
{
    public class RayGame : Game
    {
        private static readonly Random Random = new();

        public static SpriteFont DefaultFont { get; set; } = null!;
        public static Texture2D[] CompassTextures { get; } = new Texture2D[8];

        private GraphicsDeviceManager Graphics { get; }
        private Renderer GameScreen { get; set; } = null!;

        private int ViewWidth => (int)(4f*Graphics.PreferredBackBufferHeight / 3f);
        private int ViewHeight => Graphics.PreferredBackBufferHeight;

        private FramesPerSecondCounter FramesPerSecondCounter { get; } = new();

        private Map Map { get; set; } = null!;
        private Step? Step { get; set; }
        private Player Player { get; } = new Player(Def.Empty)
        {
            Location = (2.5, 2.5),
            Direction = (1, 0),
            Plane = (0, .66),
        };

        private List<WallDef> WallDefs { get; } = new List<WallDef>();

        private KeyboardState PreviousState { get; set; }

        public RayGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            GameScreen = new Renderer(
                new SpriteBatch(GraphicsDevice),
                new PrimitiveBatch(GraphicsDevice, 1000000)
            );

            // 0
            WallDefs.Add(new WallDef("Red Bricks",
                northSouthTexture: Content.Load<RayTexture>("Walls/RedBricks0"),
                eastWestTexture: Content.Load<RayTexture>("Walls/RedBricks1")
            ));

            // 1
            WallDefs.Add(new WallDef("Red Bricks Multicolored",
                northSouthTexture: Content.Load<RayTexture>("Walls/RedBricksMulticolored0"),
                eastWestTexture: Content.Load<RayTexture>("Walls/RedBricksMulticolored1")
            ));

            // 2
            WallDefs.Add(new WallDef("Red Bricks Planet",
                northSouthTexture: Content.Load<RayTexture>("Walls/RedBricksPlanet0"),
                eastWestTexture: Content.Load<RayTexture>("Walls/RedBricksPlanet1")
            ));

            // 3
            WallDefs.Add(new WallDef("Brown Bricks",
                northSouthTexture: Content.Load<RayTexture>("Walls/GreyBricks0"),
                eastWestTexture: Content.Load<RayTexture>("Walls/GreyBricks1")
            ));

            // 4
            WallDefs.Add(new WallDef("Wooden Wall",
                northSouthTexture: Content.Load<RayTexture>("Walls/WoodenWall0"),
                eastWestTexture: Content.Load<RayTexture>("Walls/WoodenWall1")
            ));

            // 5
            WallDefs.Add(new WallDef("Wooden Wall with Sign",
                northSouthTexture: Content.Load<RayTexture>("Walls/WoodenWallWithSign0"),
                eastWestTexture: Content.Load<RayTexture>("Walls/WoodenWallWithSign1")
            ));

            DefaultFont = Content.Load<SpriteFont>("DefaultFont");

            for (int i = 0; i < 8; i++)
                CompassTextures[i] = Content.Load<Texture2D>($"Ux/Compass/{i}");

            var well = new StaticObjectDef(
                name: "GreyWellFull", blocking: true,
                Content.Load<RayTexture>("Sprites/Static/GreyWellFull"));
            var bloodyWell = new StaticObjectDef(
                name: "GreyWellBlood", blocking: true,
                Content.Load<RayTexture>("Sprites/Static/GreyWellBlood"));
            var vines = new StaticObjectDef(
                name: "Vines", blocking: false,
                Content.Load<RayTexture>("Sprites/Static/Vines"));

            var rat = new ActorDef("Rat", Content.Load<RayTexture>("Sprites/Actors/Rat/1"));
            var melon = new ActorDef("Melon", Content.Load<RayTexture>("Sprites/Actors/WaterMelon/1"));
            var atmBucket = new ActorDef("Atm", Content.Load<RayTexture>("Sprites/Actors/AtmBucket/1"));

            Map = new Map(24, 24,
                    simpleMap: @"000000000000000000000000
                                 0                      0
                                 0        *             0
                                 0                      0
                                 0     0000000004 4 4   0
                                 0r+   0       0        0
                                 0     0 a     0 a  5   0
                                 0     0       0        0
                                 0    *00~0000004 4 4   0
                                 0                      0
                                 0                      0
                                 0                      0
                                 0                      0
                                 0                      0
                                 0                      0
                                 0                      0
                                 03~333333              0
                                 03 3    3  a           0
                                 03 | 2 x3              0
                                 03 3   a3              0
                                 03 333333              0
                                 03a     ~              0
                                 033333333           a  0
                                 000000000000000000000000",
                    generator: (Map map, int i, int j, char c) =>
                    {
                        if (char.IsDigit(c))
                        {
                            var wall = int.Parse(c.ToString());
                            //if (wall == 0 && Random.NextDouble() > .8)
                            //    wall = 1;
                            map.SetWall(0, i, j, WallDefs[wall]);
                        }
                        else if (c == '*')
                            map.SpawnObject(0, i, j, well);
                        else if (c == '~')
                            map.SpawnObject(0, i, j, vines).Direction = GameVector.North;
                        else if (c == '|')
                            map.SpawnObject(0, i, j, vines).Direction = GameVector.East;
                        else if (c == 'x')
                            map.SpawnObject(0, i, j, bloodyWell);
                        //else if (c == 'r')
                        //    map.SpawnActor<Wanderer>(0, i, j, rat);
                        //else if (c == '+')
                        //    map.SpawnActor<Wanderer>(0, i, j, melon);
                        //else if (c == 'a')
                        //    map.SpawnActor<Follower>(0, i, j, atmBucket);
                        else
                            map.SetWall(0, i, j, WallDef.Empty);
                    });
        }

        protected override void Update(GameTime gameTime)
        {
            Player.Update();
            if (Step != null)
                PreviousState = HandleKeyboardInput();
            Step = Map.Update(Player, ViewWidth, ViewHeight, Player.Direction / 2);

            FramesPerSecondCounter.Update(gameTime);
            base.Update(gameTime);
        }

        private KeyboardState HandleKeyboardInput()
        {
            var keyboardState = Keyboard.GetState();

            (var posX, var posY) = Player.Location.Floor();
            (var dirX, var dirY) = Player.Direction;
            (var planeX, var planeY) = Player.Plane;
            var oldDirX = dirX;
            var rotSpeed = -1.57/2;
            var moveSpeed = 1.0;

            if (keyboardState.WasKeyJustPressed(PreviousState, Keys.W, Keys.Up, Keys.J, Keys.NumPad8))
            {
                posX += (dirX).Round(0) * moveSpeed;
                posY += (dirY).Round(0) * moveSpeed;
            }
            else if (keyboardState.WasKeyJustPressed(PreviousState, Keys.S, Keys.Down, Keys.K, Keys.NumPad2))
            {
                posX -= (dirX).Round(0) * moveSpeed;
                posY -= (dirY).Round(0) * moveSpeed;
            }
            else if (keyboardState.WasKeyJustPressed(PreviousState, Keys.A, Keys.Left, Keys.H, Keys.NumPad4))
            {
                dirX = (dirX * (-rotSpeed).Cos()) - (dirY * (-rotSpeed).Sin());
                dirY = (oldDirX * (-rotSpeed).Sin()) + (dirY * (-rotSpeed).Cos());
                var oldPlaneX = planeX;
                planeX = (planeX * (-rotSpeed).Cos()) - (planeY * (-rotSpeed).Sin());
                planeY = (oldPlaneX * (-rotSpeed).Sin()) + (planeY * (-rotSpeed).Cos());
            }
            else if (keyboardState.WasKeyJustPressed(PreviousState, Keys.D, Keys.Right, Keys.L, Keys.NumPad6))
            {
                dirX = (dirX * rotSpeed.Cos()) - (dirY * rotSpeed.Sin());
                dirY = (oldDirX * rotSpeed.Sin()) + (dirY * rotSpeed.Cos());
                var oldPlaneX = planeX;
                planeX = (planeX * rotSpeed.Cos()) - (planeY * rotSpeed.Sin());
                planeY = (oldPlaneX * rotSpeed.Sin()) + (planeY * rotSpeed.Cos());
            }

            if (!Map.BlockedAt(0, (int)posX, (int)posY))
            {
                Map.ClearPlayer(Player);
                Player.Location = (posX, posY).Floor().Add(.5);
                Map.SetPlayer(Player);
            }
            Player.Direction = (dirX, dirY).Round();
            Player.Plane = (planeX, planeY).Round();


            return keyboardState;
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.Black);
            GameScreen.Draw(0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight, 0, 0, 1,
                activeRederer => activeRederer
                    .RenderWorld(ViewWidth, ViewHeight, Step!)
                    .RenderScreenFlash(ViewWidth, ViewHeight, Player)
                    .DrawText($"{Player.Location} {Player.Direction} {Player.Plane} {GameVector.CardinalDirections8Names[Player.Direction.CardinalDirection8Index]}", 0, 0, 255, 200, 200, 255)

            );
            GameScreen.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            var compass = CompassTextures[Player.Direction.CardinalDirection8Index];
            GameScreen.SpriteBatch.Draw(compass, new Rectangle(ViewWidth + 32, 32, 64, 64), Color.White);
            GameScreen.SpriteBatch.End();

            FramesPerSecondCounter.Draw(gameTime);
            Window.Title = $"RayEngine - FPS: {FramesPerSecondCounter.FramesPerSecond}";
            base.Draw(gameTime);
        }
    }
}
