using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RayEngine
{
    public static class FluentExtensions
    {
        public static T FluentWrap<T>(this T item, Action<T> action)
        {
            action(item);
            return item;
        }

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
    }
}
