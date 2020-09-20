using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.VectorDraw;
using RayLib;
using System;

namespace RayEngine
{
    class Renderer : IRenderer, IActiveRenderer
    {
        private (int w, int h) Size { get; }
        private Texture2D Canvas { get; }
        private uint[] Data { get; }

        public SpriteBatch SpriteBatch { get; }
        public PrimitiveBatch PrimitiveBatch { get; }
        public PrimitiveDrawing PrimitiveDrawing { get; }
        
        public Renderer(GraphicsDevice graphics, int viewWidth, int viewHeight)
        {
            Size = (viewWidth, viewHeight);
            SpriteBatch = new SpriteBatch(graphics);
            PrimitiveBatch = new PrimitiveBatch(graphics, 1000000);
            PrimitiveDrawing = new PrimitiveDrawing(PrimitiveBatch);
            Canvas = new Texture2D(graphics, viewWidth, viewHeight);
            Data = new uint[viewWidth * viewHeight];
        }

        public void Draw(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, Action<IActiveRenderer> action)
        {
            var viewMatrix = Matrix.Identity;
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, transformMatrix: viewMatrix);
            PrimitiveBatch.Begin(ref projectionMatrix, ref viewMatrix);
            Array.Fill(Data, 0u);
            action(this);
            Canvas.SetData(Data);
            SpriteBatch.Draw(Canvas, new Vector2(0, 0), Color.White);
            PrimitiveBatch.End();
            SpriteBatch.End();
        }

        public IActiveRenderer DrawHorizonalLine(int x, int y, int w, uint color)
        {
            if (x < 0 || y < 0 || x >= Size.w || y >= Size.h || x + w > Size.w)
                return this;

            Array.Fill(Data, color, y * Size.w + x, w);
            return this;
        }

        public IActiveRenderer PlotPoint(int x, int y, uint color)
        {
            if (x < 0 || y < 0 || x >= Size.w || y >= Size.h || (color >> 24) != 0xFF)
                return this;
            Data[(int)(y * Size.w + x)] = color;
            return this;
        }

        public IActiveRenderer DrawText(object text, int x, int y, int a, int r, int g, int b)
        {
            SpriteBatch.DrawString(RayGame.DefaultFont, text.ToString(), new(x, y), Color.FromNonPremultiplied(r, g, b, a));
            return this;
        }
    }
}
