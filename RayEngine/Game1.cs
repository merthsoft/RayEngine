using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.VectorDraw;
using RayLib;
using RayLib.Defs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RayEngine
{
    public class Game1 : Game
    {
        public static SpriteFont DefaultFont { get; set; } = null!;
        public static Texture2D[] CompassTextures { get; } = new Texture2D[4];

        private GraphicsDeviceManager Graphics { get; }
        private Renderer GameScreen { get; set; } = null!;

        private int ViewWidth => (int)(4f*Graphics.PreferredBackBufferHeight / 3f);
        private int ViewHeight => Graphics.PreferredBackBufferHeight;

        private List<Intersection> Intersections { get; } = new();
        private FramesPerSecondCounter FramesPerSecondCounter { get; } = new();

        private Map Map { get; } = new(24, 24, 1);
        private GameObject Player { get; } = new GameObject(Def.Empty)
        {
            Location = (1.5, 1.5),
            Direction = (1, 0),
            Plane = (0, .66),
        };

        private List<WallDef> WallDefs { get; } = new List<WallDef>();

        private KeyboardState PreviousState { get; set; }

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        private void StringToMap(string s)
        {
            var well = new StaticObjectDef("GreyWellFull", Content.Load<RayTexture>("Sprites/Static/GreyWellFull"));
            var bloodyWell = new StaticObjectDef("GreyWellBlood", Content.Load<RayTexture>("Sprites/Static/GreyWellBlood"));
            var vines = new StaticObjectDef("Vines", Content.Load<RayTexture>("Sprites/Static/Vines"));

            var i = 0;
            foreach (var line in s.Split('\n').Select(s => s.Trim()))
            {
                var j = 0;
                foreach (var c in line)
                {
                    if (char.IsDigit(c))
                        Map[0, i, j] = WallDefs[0];  // TODO: Parse lol
                    else if (c == '*')
                        Map.SpawnObject(i, j, well);
                    else if (c == '~')
                        Map.SpawnObject(i, j, vines);
                    else if (c == 'x')
                        Map.SpawnObject(i, j, bloodyWell);
                    else
                        Map[0, i, j] = WallDef.Empty;
                    j++;
                }
                i++;
            }
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

            WallDefs.Add(new WallDef("Red Bricks",
                northSouthTexture: Content.Load<RayTexture>("Walls/RedBricks0"),
                eastWestTexture: Content.Load<RayTexture>("Walls/RedBricks1")
            ));

            DefaultFont = Content.Load<SpriteFont>("DefaultFont");

            CompassTextures[0] = Content.Load<Texture2D>("Ux/Compass/2");
            CompassTextures[1] = Content.Load<Texture2D>("Ux/Compass/4");
            CompassTextures[2] = Content.Load<Texture2D>("Ux/Compass/1");
            CompassTextures[3] = Content.Load<Texture2D>("Ux/Compass/3");


            StringToMap(
                @"111111111111111111111111
                  1                      1
                  1        *             1
                  1                      1
                  1     22222    3 3 3   1
                  1     2   2            1
                  1     2   2    3   3   1
                  1     2   2            1
                  1    *22 22    3 3 3   1
                  1                      1
                  1                      1
                  1                      1
                  1                      1
                  1                      1
                  1                      1
                  1                      1
                  14~444444              1
                  14 4    4              1
                  14 ~ 5 x4              1
                  14 4    4              1
                  14 444444              1
                  14      ~              1
                  144444444              1
                  111111111111111111111111");
            


            Intersections.AddRange(Map.GenerateIntersections(Player, ViewWidth, ViewHeight));
        }

        protected override void Update(GameTime gameTime)
        {
            PreviousState = HandleKeyboardInput();
            Intersections.Clear();
            Intersections.AddRange(Map.GenerateIntersections(Player, ViewWidth, ViewHeight));

            FramesPerSecondCounter.Update(gameTime);
            base.Update(gameTime);
        }

        private KeyboardState HandleKeyboardInput()
        {
            var keyboardState = Keyboard.GetState();

            (var posX, var posY) = Player.Location;
            (var dirX, var dirY) = Player.Direction;
            (var planeX, var planeY) = Player.Plane;
            var oldDirX = dirX;
            var rotSpeed = 1.57;
            var moveSpeed = 1.0;

            if (keyboardState.WasKeyJustPressed(PreviousState, Keys.W, Keys.Up, Keys.J, Keys.NumPad8))
            {
                posX += dirX * moveSpeed;
                posY += dirY * moveSpeed;
            }
            else if (keyboardState.WasKeyJustPressed(PreviousState, Keys.S, Keys.Down, Keys.K, Keys.NumPad2))
            {
                posX -= dirX * moveSpeed;
                posY -= dirY * moveSpeed;
            }
            else if (keyboardState.WasKeyJustPressed(PreviousState, Keys.NumPad9))
            {
                posX += (planeX * moveSpeed).Round(0);
                posY += (planeY * moveSpeed).Round(0);
            }
            else if (keyboardState.WasKeyJustPressed(PreviousState, Keys.NumPad7))
            {
                posX -= (planeX * moveSpeed).Round(0);
                posY -= (planeY * moveSpeed).Round(0);
            }
            else if (keyboardState.WasKeyJustPressed(PreviousState, Keys.A, Keys.Left, Keys.H, Keys.NumPad4))
            {
                dirX = dirX * -rotSpeed.Cos() - dirY * -rotSpeed.Sin();
                dirY = oldDirX * -rotSpeed.Sin() + dirY * -rotSpeed.Cos();
                var oldPlaneX = planeX;
                planeX = planeX * -rotSpeed.Cos() - planeY * -rotSpeed.Sin();
                planeY = oldPlaneX * -rotSpeed.Sin() + planeY * -rotSpeed.Cos();
            }
            else if (keyboardState.WasKeyJustPressed(PreviousState, Keys.D, Keys.Right, Keys.L, Keys.NumPad6))
            {
                dirX = dirX * rotSpeed.Cos() - dirY * rotSpeed.Sin();
                dirY = oldDirX * rotSpeed.Sin() + dirY * rotSpeed.Cos();
                var oldPlaneX = planeX;
                planeX = planeX * rotSpeed.Cos() - planeY * rotSpeed.Sin();
                planeY = oldPlaneX * rotSpeed.Sin() + planeY * rotSpeed.Cos();
            }

            if (Map[0, posX, posY] == WallDef.Empty)
            {
                Player.Location = (posX, posY).Round();
                Player.Direction = (dirX, dirY).Round();
                Player.Plane = (planeX, planeY).Round();
            }


            return keyboardState;
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.Black);
            GameScreen.Draw(0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight, 0, 0, 1,
                activeRederer => activeRederer
                    .RenderWorld(ViewWidth, ViewHeight, Intersections)
                    .DrawText(Player.Plane, 0, 0, 150, 150, 150)
            );
            GameScreen.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            var compass = Player.Direction.Atan2() switch
            {
                var x when x == -MathF.PI / 2f => CompassTextures[3],
                var x when x < MathF.PI / 2f => CompassTextures[0],
                var x when x < MathF.PI => CompassTextures[1],
                var x when x < 3f * MathF.PI / 2f => CompassTextures[2],
                _ => CompassTextures[3]
            };
            GameScreen.SpriteBatch.Draw(compass, new Rectangle(ViewWidth + 32, 32, 64, 64), Color.White);
            GameScreen.SpriteBatch.End();

            FramesPerSecondCounter.Draw(gameTime);
            Window.Title = $"RayEngine - FPS: {FramesPerSecondCounter.FramesPerSecond}";
            base.Draw(gameTime);
        }
    }
}
