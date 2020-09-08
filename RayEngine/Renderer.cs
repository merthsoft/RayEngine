using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.VectorDraw;
using RayLib;
using System;

namespace RayEngine
{
    class Renderer : IRenderer, IActiveRenderer
    {
        public SpriteBatch SpriteBatch { get; }
        public PrimitiveBatch PrimitiveBatch { get; }
        public PrimitiveDrawing PrimitiveDrawing { get; }

        public Renderer(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
            => (SpriteBatch, PrimitiveBatch, PrimitiveDrawing)
             = (spriteBatch, primitiveBatch, new PrimitiveDrawing(primitiveBatch));

        public void Draw(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, Action<IActiveRenderer> action)
        {
            var viewMatrix = Matrix.Identity;
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, transformMatrix: viewMatrix);
            PrimitiveBatch.Begin(ref projectionMatrix, ref viewMatrix);
            action(this);
            PrimitiveBatch.End();
            SpriteBatch.End();
        }

        public IActiveRenderer DrawLine(float x1, float y1, float x2, float y2, int a, int r, int g, int b)
        {
            PrimitiveDrawing.DrawSegment(new(x1, y1), new(x2, y2), Color.FromNonPremultiplied(r, g, b, a));
            return this;
        }

        public IActiveRenderer PlotPoint(float x, float y, int a, int r, int g, int b)
        {
            PrimitiveDrawing.DrawSegment(new(x, y), new(x + 1, y + 1), new(r, g, b, a));
            return this;
        }

        public IActiveRenderer DrawText(object text, float x, float y, int a, int r, int g, int b)
        {
            SpriteBatch.DrawString(RayGame.DefaultFont, text.ToString(), new(x, y), Color.FromNonPremultiplied(r, g, b, a));
            return this;
        }
    }
}
