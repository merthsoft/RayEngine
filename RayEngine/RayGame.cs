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
using System.Linq;
using static RayEngine.Actors.GamePlayer;

namespace RayEngine
{
    public class RayGame : Game
    {
        public static SpriteFont DefaultFont { get; private set; } = null!;
        public static Texture2D CompassBase { get; private set; } = null!;
        public static Texture2D CompassNeedle { get; private set; } = null!;
        public static Dictionary<FaceState, Texture2D> FaceTextures { get; } = new();

        private GraphicsDeviceManager Graphics { get; }
        private Renderer GameScreen { get; set; } = null!;

        private int ViewWidth => (int)(4f*Graphics.PreferredBackBufferHeight / 3f);
        private int ViewHeight => Graphics.PreferredBackBufferHeight;

        private FramesPerSecondCounter FramesPerSecondCounter { get; } = new();

        private Map Map { get; set; } = null!;
        private Step? Step { get; set; }
        private GamePlayer Player { get; } = new GamePlayer(ActorDef.Empty)
        {
            Location = (2.5, 2.5),
            Direction = GameVector.East,
            FieldOfView = .75,
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

            CompassBase = Content.Load<Texture2D>($"Ux/Compass/Base");
            CompassNeedle = Content.Load<Texture2D>($"Ux/Compass/Needle");

            foreach (var state in Enum.GetValues(typeof(FaceState)).Cast<FaceState>())
                FaceTextures[state] = Content.Load<Texture2D>($"Ux/Face/{state}");

            var well = new StaticObjectDef(
                name: "GreyWellFull", blocking: true, blocksView: false,
                Content.Load<RayTexture>("Sprites/Static/GreyWellFull"));
            var bloodyWell = new StaticObjectDef(
                name: "GreyWellBlood", blocking: true, blocksView: false,
                Content.Load<RayTexture>("Sprites/Static/GreyWellBlood"));
            var vines = new StaticObjectDef(
                name: "Vines", blocking: false, blocksView: true,
                Content.Load<RayTexture>("Sprites/Static/Vines/0"),
                Content.Load<RayTexture>("Sprites/Static/Vines/1"));
            var bucket = new StaticObjectDef(
                name: "Bucket", blocking: true, blocksView: false,
                Content.Load<RayTexture>("Sprites/Static/Bucket"));
            var pillar = new StaticObjectDef(
                name: "Pillar", blocking: true, blocksView: false,
                Content.Load<RayTexture>("Sprites/Static/Pillar"));

            var spider = new ActorDef("Spider", blocksView: false, 
                Content.Load<RayTexture>("Sprites/Actors/Spider/1"));

            var atmBucket = new ActorDef("Atm", blocksView: false,
                Content.Load<RayTexture>("Sprites/Actors/AtmBucket/Idle"),
                Content.Load<RayTexture>("Sprites/Actors/AtmBucket/Wakeup"),
                Content.Load<RayTexture>("Sprites/Actors/AtmBucket/Walk")
            );

            Map = new Map(ViewWidth, ViewHeight,
                    simpleMap: @"0000000000000000000000000000000000000000000000000000
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0~00000000000000000000000                          0
                                 0v   I     I    BI      0                          0
                                 0                       0                          0
                                 0         B    B        0                          0
                                 0B B                B   0                          0
                                 0       B               0                          3
                                 0    I     I     I      0                          3
                                 0                       0                          3
                                 0      BB     B         0                          3
                                 0                       |                          3
                                 0BBBB BBBB  BBBB  BBBBBB0                          0
                                 0BBBBIBBBBBIBBBBBIBBBBBB0                          0
                                 0000000000000000000000000000000000000000000000000000",
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
                        else if (c == 'B')
                            map.SpawnObject(0, i, j, bucket);
                        else if (c == 'I')
                            map.SpawnObject(0, i, j, pillar);
                        else if (c == 's')
                            map.SpawnActor<Wanderer>(0, i, j, spider).Direction = GameVector.East;
                        else if (c == 'a')
                            map.SpawnActor<Sleeper>(0, i, j, atmBucket)
                                .SetDirection(GameVector.South)
                                .SetFieldOfView(2);
                        else if ("<^>v".Contains(c))
                            Player
                                .SetLocation((i, j))
                                .SetDirection(GameVector.CardinalDirections4["<^>v".IndexOf(c)]);
                        else
                            map.SetWall(0, i, j, WallDef.Empty);
                    });
        }

        protected override void Update(GameTime gameTime)
        {
            Player.Act(Map, Player);
            if (Step != null)
                PreviousState = HandleKeyboardInput();
            Step = Map.Update(Player, Player.Direction / 2);

            FramesPerSecondCounter.Update(gameTime);
            base.Update(gameTime);
        }

        private KeyboardState HandleKeyboardInput()
        {
            var keyboardState = Keyboard.GetState();

            (var posX, var posY) = Player.Location.Floor();
            (var dirX, var dirY) = Player.Direction;
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
            }
            else if (keyboardState.WasKeyJustPressed(PreviousState, Keys.D, Keys.Right, Keys.L, Keys.NumPad6))
            {
                dirX = (dirX * rotSpeed.Cos()) - (dirY * rotSpeed.Sin());
                dirY = (oldDirX * rotSpeed.Sin()) + (dirY * rotSpeed.Cos());
            }
            else if (keyboardState.IsKeyDown(Keys.OemPlus))
                Player.FieldOfView = (Player.FieldOfView + .01).Round(2);
            else if (keyboardState.IsKeyDown(Keys.OemMinus) && Player.FieldOfView > .01)
                Player.FieldOfView = (Player.FieldOfView - .01).Round(2);

            if (!Map.BlockedAt(0, (int)posX, (int)posY))
            {
                Map.ClearPlayer(Player);
                Player.Location = (posX, posY).Floor().Add(.5);
                Map.SetPlayer(Player);
            }
            Player.Direction = (dirX, dirY).Round();

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
            GameScreen.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            
            var midX = ViewWidth + (Graphics.PreferredBackBufferWidth - ViewWidth) / 2;
            var face = FaceTextures[0];
            GameScreen.SpriteBatch
                .DrawTexture(CompassBase, midX - 75, 32, 150, 150)
                .DrawTexture(CompassNeedle, midX - 75, 32, 150, 150, -Player.Direction.Atan2())
                .DrawTexture(FaceTextures[Player.Face], midX - 75, ViewHeight - 160, 150, 150)
            ;
            GameScreen.SpriteBatch.End();

            FramesPerSecondCounter.Draw(gameTime);
            Window.Title = $"RayEngine - FPS: {FramesPerSecondCounter.FramesPerSecond}";
            base.Draw(gameTime);
        }
    }
}
