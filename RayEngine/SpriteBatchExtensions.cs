using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RayEngine.Actors;
using System;

namespace RayEngine
{
    public static class SpriteBatchExtensions
    {
        public static SpriteBatch DrawTexture(this SpriteBatch sb, Texture2D texture, int x, int y, int w, int h, double rotation = 0.0)
        {
            var center = Vector2.Zero;
            if (rotation != 0)
            {
                center = new(texture.Width / 2, texture.Height / 2);
                x += w / 2;
                y += h / 2;
            }
            sb.Draw(texture, new Rectangle(x, y, w, h), texture.Bounds, Color.White, (float)rotation, center, SpriteEffects.None, 0);
            return sb;
        }

        public static SpriteBatch RenderScreenFlash(this SpriteBatch sb, int viewWidth, int viewHeight, GamePlayer player)
        {
            if (player.ScreenFlashDuration == 0)
                return sb;
            (var r, var g, var b) = player.ScreenFlash;
            var a = Math.Min(240, player.ScreenFlashDuration);
            sb.DrawRectangle(0, 0, viewWidth, viewHeight, new Color((uint)(a << 24 | r << 16 | g << 8 | b)));
            return sb;
        }
    }
}
