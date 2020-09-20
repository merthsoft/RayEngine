using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RayLib;
using System;

namespace RayEngine
{
    class Renderer : IRenderer<Renderer>, IActiveRenderer
    {
        private (int w, int h) Size { get; }
        private Texture2D Canvas { get; }
        private uint[] Data { get; }

        public SpriteBatch SpriteBatch { get; }
        
        public Renderer(GraphicsDevice graphics, int viewWidth, int viewHeight)
        {
            Size = (viewWidth, viewHeight);
            SpriteBatch = new SpriteBatch(graphics);
            Canvas = new Texture2D(graphics, viewWidth, viewHeight);
            Data = new uint[viewWidth * viewHeight];
        }

        public void Draw(Action<Renderer> action)
        {
            var viewMatrix = Matrix.Identity;
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, transformMatrix: viewMatrix);
            Array.Fill(Data, 0u);
            action(this);
            Canvas.SetData(Data);
            SpriteBatch.Draw(Canvas, new Vector2(0, 0), Color.White);
            SpriteBatch.End();
        }

        public Renderer DrawHorizonalLine(int x, int y, int w, uint color)
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
            Data[y * Size.w + x] = color;
            return this;
        }
    }
}
