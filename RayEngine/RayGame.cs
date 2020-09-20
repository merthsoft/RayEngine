using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using RayEngine.Actors;
using RayLib;
using RayLib.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using static RayEngine.Actors.GamePlayer;
using RayLib.Extensions;

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
            IsMouseVisible = false;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            GameScreen = new Renderer(Graphics.GraphicsDevice, ViewWidth, ViewHeight);

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
                name: "Vines", blocking: false, blocksView: false,
                Content.Load<RayTexture>("Sprites/Static/Vines/0"),
                Content.Load<RayTexture>("Sprites/Static/Vines/1"));
            var bucket = new StaticObjectDef(
                name: "Bucket", blocking: true, blocksView: false,
                Content.Load<RayTexture>("Sprites/Static/Bucket"));
            var pillar = new StaticObjectDef(
                name: "Pillar", blocking: true, blocksView: false,
                Content.Load<RayTexture>("Sprites/Static/Pillar"));
            
            var door = new ActorDef(name: "Door", 
                new[] { Content.Load<RayTexture>("Sprites/Interactable/WoodenDoor/Closed") },
                new[] { Content.Load<RayTexture>("Sprites/Interactable/WoodenDoor/Opening") },
                new[] { Content.Load<RayTexture>("Sprites/Interactable/WoodenDoor/Open") }) { RenderStyle = RenderStyle.Wall};

            var spider = new ActorDef("Spider", new[] { Content.Load<RayTexture>("Sprites/Actors/Spider/1") });

            var atmBucket = new ActorDef("Atm", new[] { Content.Load<RayTexture>("Sprites/Actors/AtmBucket/Idle") },
                new[] { Content.Load<RayTexture>("Sprites/Actors/AtmBucket/Wakeup") },
                new[] { Content.Load<RayTexture>("Sprites/Actors/AtmBucket/Walk") }
            );

            Map = new Map(ViewWidth, ViewHeight,
                    simpleMap: @"0000000000000000000000000000000000000000000000000000
                                 0  v  0                                            0
                                 0     0                                            0
                                 0 I I 0                                            0
                                 000-0000000033333                                  0
                                 0     0   I3    3                                  0
                                 0  B  |    |   a3                                  0
                                 0     0   I3    3                                  0
                                 00000000-00333333                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  0
                                 0                                                  3
                                 0                                                  3
                                 0                                                  3
                                 0                                                  3
                                 0                                                  3
                                 0                                                  0
                                 0                                                  0
                                 0000000000000000000000000000000000000000000000000000",
                    generator: (Map map, int i, int j, char c, char[,] neighbors) =>
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
                        else if (c == '-' || c == '|')
                        {
                            var direction = c == '-' ? GameVector.East : GameVector.North;
                            var neighborWall = neighbors[(int)direction.X + 1, (int)direction.Y + 1];
                            map.SpawnActor<Door>(0, i, j, door, direction,
                                preInit: door => door.BackWall = WallDefs[int.Parse(neighborWall.ToString())]
                            );
                        }
                        else if (c == 'x')
                            map.SpawnObject(0, i, j, bloodyWell);
                        else if (c == 'B')
                            map.SpawnObject(0, i, j, bucket);
                        else if (c == 'I')
                            map.SpawnObject(0, i, j, pillar);
                        else if (c == 's')
                            map.SpawnActor<Wanderer>(0, i, j, spider, GameVector.East);
                        else if (c == 'a')
                            map.SpawnActor<Sleeper>(0, i, j, atmBucket, GameVector.West, 1);
                        else if (">^<v".Contains(c))
                            Player
                                .SetLocation(i, j)
                                .SetDirection(GameVector.CardinalDirections4[">^<v".IndexOf(c)]);
                        else
                            map.SetWall(0, i, j, WallDef.Empty);
                    });
        }

        protected override void Update(GameTime gameTime)
        {
            Player.Act(null!); // TODO: This is dirty. Use a singleton instead of null
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

            if (!Map.BlockedAt(0, (int)posX, (int)posY) || Player.NoClip)
            {
                Map.ClearPlayer(Player);
                Player.Location = (posX, posY).Floor().Add(.5);
                Map.SetPlayer(Player);
            }
            Player.Direction = (dirX, dirY).Round();

            Player.Interacting = keyboardState.IsKeyDown(Keys.Space, Keys.NumPad0);

            return keyboardState;
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            GameScreen.Draw(activeRederer => activeRederer
                    .RenderWorld(ViewWidth, ViewHeight, Step!)
                    .RenderScreenFlash(ViewWidth, ViewHeight, Player)
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
